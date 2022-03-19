import javafx.animation.AnimationTimer;
import javafx.application.Application;
import javafx.scene.Group;
import javafx.scene.Scene;
import javafx.scene.canvas.Canvas;
import javafx.scene.canvas.GraphicsContext;
import javafx.scene.paint.Color;
import javafx.stage.Stage;
import world.gui.SimulationGUICreator;
import world.simulation.SimulationParameters;
import world.simulation.Simulation;
import world.gui.TextVisualizer;


public class Main extends Application {

    public static void main(String[] args) {
        launch(args);
    }

    @Override
    public void start(Stage stage) throws InterruptedException {
        stage.setTitle("Darwin Evolution Simulator");

        Group root = new Group();
        Scene scene = new Scene(root);
        stage.setScene(scene);
        scene.setFill(Color.DARKSLATEGRAY);

        Canvas canvas = new Canvas(1300, 700);
        GraphicsContext gc = canvas.getGraphicsContext2D();
        gc.setFill(Color.BLACK);
        gc.setStroke(Color.BLACK);
        gc.setLineWidth(2);

        try {
            SimulationParameters parameters =
                    new SimulationParameters("src\\main\\java\\parameters.json");
            new TextVisualizer(10, 50, 180, 400, gc, parameters, Color.SLATEGRAY).render();

            int simulationsNumber = 2;
            Simulation[] simulations = SimulationGUICreator.create(simulationsNumber, stage, gc, parameters, root, canvas);

            scene.setOnMouseClicked(
                    event -> {
                        for (Simulation simulation : simulations)
                            simulation.clickEvent(event.getX(), event.getY(), gc);
                    }
            );

            new AnimationTimer() {
                public void handle(long currentSimulationTime) {
                    for (Simulation simulation : simulations) {
                        simulation.update();
                        simulation.render();
                    }
                    try {
                        Thread.sleep(parameters.getInterval());
                    } catch (InterruptedException e) {
                        e.printStackTrace();
                    }
                }
            }.start();

            stage.show();

        } catch (IllegalArgumentException exception) {
            root.getChildren().addAll(canvas);
            new TextVisualizer(10, 50, 180, 400, gc, exception, Color.SLATEGRAY).render();
            stage.show();
        }
    }
}