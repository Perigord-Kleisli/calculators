{
  maven,
  makeWrapper,
  stdenv,
  glib,
  xorg,
  lib,
  jdk17,
}:
let java-calc = maven.buildMavenPackage rec {
  pname = "java-calculator";
  version = "1.0.0";
  name = "${pname}-${version}";

  src = ./.;
  mvnHash = "sha256-TaCLMUv8hnF8Q53ElqXjLUtM0ar8/90lcQmh0/Gwk/A=";
  nativeBuildInputs = [makeWrapper];
  buildInputs = [stdenv.cc.cc glib xorg.libXxf86vm xorg.libXtst];

  installPhase = let
    modules = lib.strings.concatStringsSep ":" [
      "${java-calc.fetchedMavenDeps}/.m2/org/openjfx/javafx-controls/20/javafx-controls-20-linux.jar"
      "${java-calc.fetchedMavenDeps}/.m2/org/openjfx/javafx-graphics/20/javafx-graphics-20-linux.jar"
      "${java-calc.fetchedMavenDeps}/.m2/org/openjfx/javafx-base/20/javafx-base-20-linux.jar"
      "${java-calc.fetchedMavenDeps}/.m2/org/openjfx/javafx-fxml/20/javafx-fxml-20-linux.jar"
    ];
  in ''
    mkdir -p $out/bin $out/share/${pname}
    install -Dm644 target/${name}.jar $out/share/${pname}

    makeWrapper ${jdk17}/bin/java $out/bin/${pname} \
      --add-flags "--module-path $out/share/${pname}/${name}.jar:${modules} -m com.perigord/com.perigord.App" \
      --set LD_LIBRARY_PATH  "${lib.makeLibraryPath [stdenv.cc.cc glib xorg.libXxf86vm xorg.libXtst]}"
  '';

  meta = with lib; {
    description = "A Calculator written in Java";
    license = licenses.gpl3Plus;
  };
};
in java-calc

