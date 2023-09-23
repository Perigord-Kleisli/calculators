# {
#   description = "A very basic flake";
#
#   inputs = {
#     nuget-packageslock2nix = {
#       url = "github:mdarocha/nuget-packageslock2nix/main";
#       inputs.nixpkgs.follows = "nixpkgs";
#     };
#   };
#
#   outputs = {
#     nixpkgs,
#     nuget-packageslock2nix,
#     ...
#   }: let
#     pkgs = import nixpkgs {system = "x86_64-linux";};
#
#     project = with pkgs;
{
  buildDotnetModule,
  nuget-packageslock2nix,
  gtk3,
}:
buildDotnetModule {
  pname = "csharp-calculator";
  version = "0.1";
  src = ./.;

  nugetDeps = nuget-packageslock2nix.lib {
    system = "x86_64-linux";
    lockfiles = [./packages.lock.json];
  };

  runtimeDeps = [gtk3];
}
