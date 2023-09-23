# A Bunch of calculators

```sh
├───devShells
│   └───x86_64-linux
│       ├───csharp-calculator: development environment 'nix-shell'
│       ├───java-calculator: development environment 'nix-shell'
│       └───python-calculator: development environment 'nix-shell'
└───packages
    └───x86_64-linux
        ├───csharp-calculator: package 'csharp-calculator-0.1'
        ├───java-calculator: package 'java-calculator-1.0.0'
        └───python-calculator: package 'python3.10-python-calculator-0.1.0'
```

## Running
`nix run <location>#<package>`

- i.e:
    - `nix run .#csharp-calculator` (locally)
    - `nix run github:Perigord-Kleisli/calulators#java-calculator` (from github repo)

Keep in mind that `.#python-calculator` cannot run unless you're in the `.#python-calculator` devshell. I couldn't figure out how to wrap it with gtk.

## Entering devshell
Same thing, but with `nix develop` instead of `nix run`
- i.e:
    - `nix develop .#csharp-calculator` (locally)
    - `nix develop github:Perigord-Kleisli/calulators#java-calculator` (from github repo)

