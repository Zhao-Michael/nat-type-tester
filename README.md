# NAT Type Tester

This project is a console cli port of [NatTypeTester](https://github.com/HMBSbige/NatTypeTester). Current it only has RFC3489 implemented.

## Quick Start

```
D:\projects\nat-type-tester>dotnet run --project NatTypeTester -- --help
NatTypeTester 1.0.0+9522dac1a65800707197d80876975846e19738ab
Copyright (C) 2025 NatTypeTester

  -s, --stun        (Default: stun.fitauto.ru) Stun Server.

  -p, --proxy       Socks5 Proxy.

  -u, --user        Socks5 Proxy Username.

  -c, --password    Socks5 Proxy Password.

  --help            Display this help screen.

  --version         Display version information.
```

Avaible Stun server list according to https://github.com/HMBSbige/NatTypeTester/commit/65f15fddaf488a5b06080bd8a641a50a1a04407e

- stun.hot-chilli.net
- stun.fitauto.ru
- stun.internetcalls.com
- stun.miwifi.com
- stun.voip.aebc.com
- stun.voipbuster.com
- stun.voipstunt.com
