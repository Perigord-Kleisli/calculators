package com.perigord;

import java.io.IOException;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Scene;
import javafx.stage.Stage;
import javafx.stage.StageStyle;


public class App extends Application {


    public void start(Stage stage) throws IOException {
        FXMLLoader loader = new FXMLLoader(getClass().getResource("scene.fxml"));
        loader.setController(new Controller());

        var scene = new Scene(loader.load(), 620, 400);
        scene.getStylesheets().add(getClass().getResource("styles.css").toString());
        
        var winWidth = 623;
        var winHeight = 551;
        stage.setMinWidth(winWidth);
        stage.setMaxWidth(winWidth);
        stage.setMinHeight(winHeight);
        stage.setMaxHeight(winHeight);

        stage.setResizable(false);
        stage.initStyle(StageStyle.TRANSPARENT);
        stage.setTitle("Calculator");
        stage.setScene(scene);
        stage.show();
    }

    public static void main(String[] args) {
        launch();
    }

}