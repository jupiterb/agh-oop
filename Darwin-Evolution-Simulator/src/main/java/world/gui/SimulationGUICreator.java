package world.gui;

import javafx.scene.Group;
import javafx.scene.canvas.Canvas;
import javafx.scene.canvas.GraphicsContext;
import javafx.scene.control.Button;
import javafx.stage.Stage;
import world.simulation.SimulationParameters;
import world.simulation.Simulation;

public class SimulationGUICreator {

    private final double positionX;
    private final double positionY;
    private final double mapSize;
    private final double fontSize;
    private final SimulationParameters parameters;

    private static final String pathToAnimalImage = "images\\animal2.png";
    private static final String pathToGrassImage = "images\\grass.png";
    private static final String pathToGroundImage = "images\\ground2.jpg";
    private static final String pathToSpecialImage = "images\\animal3.png";

    private final String fileToWriteStats;

    public SimulationGUICreator(double positionX, double positionY, double mapSize, double fontSize,
                                SimulationParameters parameters, String pathToStats) {
        this.positionX = positionX;
        this.positionY = positionY;
        this.mapSize = mapSize;
        this.fontSize = fontSize;
        this.parameters = parameters;
        this.fileToWriteStats = pathToStats;
    }

    private Simulation createSimulationAndButtons(Button run, Button track, Button genome, Button save, GraphicsContext gc){
        Simulation simulation = new Simulation(parameters, gc, positionX+mapSize/3, positionY+mapSize, mapSize, fontSize,
                pathToAnimalImage, pathToGrassImage,pathToGroundImage,pathToSpecialImage);

        double buttonShift = 10;
        double buttonWidth = mapSize/3 - buttonShift;
        double buttonHeight = 40;
        double buttonDistance = (mapSize-buttonHeight) / 3;

        run.setMinWidth(buttonWidth);
        run.setMinHeight(buttonHeight);
        run.setLayoutX(positionX+buttonShift);
        run.setLayoutY(positionY);
        run.setStyle("-fx-background-color: SlateBlue");

        track.setMinWidth(buttonWidth);
        track.setMinHeight(buttonHeight);
        track.setLayoutX(positionX+buttonShift);
        track.setLayoutY(positionY+buttonDistance);
        track.setStyle("-fx-background-color: SlateBlue");

        genome.setMinWidth(buttonWidth);
        genome.setMinHeight(buttonHeight);
        genome.setLayoutX(positionX+buttonShift);
        genome.setLayoutY(positionY+buttonDistance*2);
        genome.setStyle("-fx-background-color: SlateBlue");

        save.setMinWidth(buttonWidth);
        save.setMinHeight(buttonHeight);
        save.setLayoutX(positionX+buttonShift);
        save.setLayoutY(positionY+buttonDistance*3);
        save.setStyle("-fx-background-color: SlateBlue");

        run.setOnAction(actionEvent -> {
            simulation.runnable = !simulation.runnable;
        });

        track.setOnAction(actionEvent -> {
            simulation.trackAnimal();
        });

        genome.setOnAction(actionEvent -> {
            simulation.showAnimalsWithMostCommonGenome(gc);
        });

        save.setOnAction(actionEvent -> {
            simulation.saveToFile(fileToWriteStats);
        });
        return simulation;
    }

    public static Simulation [] create(int simulationsNumber, Stage stage, GraphicsContext gc,
                                              SimulationParameters parameters, Group root, Canvas canvas){

        Simulation [] simulations = new Simulation[simulationsNumber];
        Button[] runButtons = new Button[simulationsNumber];
        Button[] trackButtons = new Button[simulationsNumber];
        Button[] genomesButtons = new Button[simulationsNumber];
        Button[] saveButtons = new Button[simulationsNumber];
        SimulationGUICreator[] setter = new SimulationGUICreator[simulationsNumber];
        for (int i = 0; i < simulationsNumber; i++) {
            runButtons[i] = new Button("Run / Pause");
            trackButtons[i] = new Button("Track Animal\n or finish tracking");
            genomesButtons[i] = new Button("Animals with most\ncommon genome");
            saveButtons[i] = new Button("Save this Simulation");
            setter[i] = new SimulationGUICreator(200 + i * 550, 50, 400, 20, parameters, "Stats" + i + ".txt");
            simulations[i] = setter[i].createSimulationAndButtons(runButtons[i], trackButtons[i],
                    genomesButtons[i], saveButtons[i], gc);
            simulations[i].renderMap();
        }
        Button escapeButton = createEscapeButton(stage);
        root.getChildren().addAll(canvas, escapeButton,
                runButtons[0], trackButtons[0], genomesButtons[0], saveButtons[0],
                runButtons[1], trackButtons[1], genomesButtons[1], saveButtons[1]);

        return simulations;
    }

    private static Button createEscapeButton(Stage stage){
        Button escapeButton = new Button("Escape");
        escapeButton.setMinWidth(400.0 / 300.0 - 10);
        escapeButton.setMinHeight(40);
        escapeButton.setLayoutX(50);
        escapeButton.setLayoutY(500);
        escapeButton.setStyle("-fx-background-color: SlateBlue");
        escapeButton.setOnAction(actionEvent -> {
            stage.close();
        });
        return escapeButton;
    }
}
