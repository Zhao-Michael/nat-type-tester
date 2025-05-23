using CommandLine;
using Dns.Net.Clients;
using Socks5.Models;
using STUN;
using STUN.Client;
using STUN.Enums;
using STUN.Proxy;
using System.Net;
using System.Net.Sockets;

namespace NatTypeTester;

public class Options
{
    [Option('s', "stun", Default = "stun.fitauto.ru", Required = false, HelpText = "Stun Server.")]
    public string StunServer { get; set; } = string.Empty;

    [Option('p', "proxy", Default = null, Required = false, HelpText = "Socks5 Proxy.")]
    public string? ProxyServer { get; set; }

    [Option('u', "user", Default = null, Required = false, HelpText = "Socks5 Proxy Username.")]
    public string? ProxyUser { get; set; }

    [Option('c', "password", Default = null, Required = false, HelpText = "Socks5 Proxy Password.")]
    public string? ProxyPass { get; set; }
}

class Program
{
    static async Task Main(string[] args)
    {
        var result = Parser.Default.ParseArguments<Options>(args);
        Options? options = null;
        result
            .MapResult(
                opts => options = opts,
                _ => options = null);

        if (options is null)
        {
            return;
        }

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) =>
        {
            Console.WriteLine("Canceling...");
            cts.Cancel();
            e.Cancel = true;
        };

        Console.WriteLine($"Testing NAT Type with {options.ProxyServer ?? "no proxy"}...");
        await TestClassicNatTypeAsync(options.StunServer, options.ProxyServer, options.ProxyUser, options.ProxyPass, cts.Token);
    }
    private static async Task TestClassicNatTypeAsync(string stunServer, string? proxyServer, string? proxyUser, string? proxyPass, CancellationToken token)
    {
        var DnsClient = new DefaultDnsClient();

        if (!StunServer.TryParse(stunServer, out StunServer? server))
        {
            throw new NotSupportedException(@"Wrong STUN Server!");
        }

        var proxyType = proxyServer is not null ? ProxyType.Socks5 : ProxyType.Plain;
        Socks5CreateOption socks5Option = new();
        if (proxyServer is not null)
        {
            if (!HostnameEndpoint.TryParse(proxyServer, out HostnameEndpoint? proxyIpe) || proxyIpe.Port == 0)
            {
                throw new NotSupportedException(@"Unknown proxy address!");
            }
            socks5Option.Address = await DnsClient.QueryAsync(proxyIpe.Hostname, token);
            socks5Option.Port = proxyIpe.Port;
            socks5Option.UsernamePassword = new UsernamePassword
            {
                UserName = proxyUser,
                Password = proxyPass,
            };
        }

        Console.WriteLine($"STUN Server: {server.Hostname}:{server.Port}");
        var serverIp = await DnsClient.QueryAsync(server.Hostname, token);
        var localEndPoint = serverIp.AddressFamily is AddressFamily.InterNetworkV6 ? new IPEndPoint(IPAddress.IPv6Any, IPEndPoint.MinPort) : new IPEndPoint(IPAddress.Any, IPEndPoint.MinPort);

        using IUdpProxy proxy = ProxyFactory.CreateProxy(proxyType, localEndPoint, socks5Option);
        using StunClient3489 client = new(new IPEndPoint(serverIp, server.Port), localEndPoint, proxy);

        try
        {
            await client.ConnectProxyAsync(token);
            try
            {
                await client.QueryAsync(token);
            }
            finally
            {
                await client.CloseProxyAsync(token);
            }
        }
        finally
        {
            var Result3489 = client.State with { };
            Console.WriteLine($"STUN Result: {Result3489.NatType}");
        }
    }
}
