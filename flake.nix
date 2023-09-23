{
  description = "A very basic flake";

  inputs = {
    nuget-packageslock2nix = {
      url = "github:mdarocha/nuget-packageslock2nix/main";
      inputs.nixpkgs.follows = "nixpkgs";
    };
  };

  outputs = {
    nixpkgs,
    nuget-packageslock2nix,
    ...
  }: let
    pkgs = import nixpkgs {system = "x86_64-linux";};
  in {
    packages.x86_64-linux.java-calculator = pkgs.callPackage ./java-calculator {};
    devShells.x86_64-linux.java-calculator = pkgs.callPackage ./java-calculator/shell.nix {};

    packages.x86_64-linux.csharp-calculator = pkgs.callPackage ./csharp-calculator {inherit nuget-packageslock2nix; };
    devShells.x86_64-linux.csharp-calculator = pkgs.callPackage ./csharp-calculator/shell.nix {};

    packages.x86_64-linux.python-calculator = pkgs.callPackage ./python-calculator {};
    devShells.x86_64-linux.python-calculator = pkgs.callPackage ./python-calculator/shell.nix {};
  };
}
