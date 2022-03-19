package world.simulation;

import javafx.scene.canvas.GraphicsContext;
import javafx.scene.paint.Color;
import world.elements.Animal;
import world.gui.IView;
import world.gui.JungleSteppeVisualizer;
import world.gui.TextVisualizer;
import world.map.JungleSteppe;
import world.simulation.engine.DarwinEvoSimulationEngine;
import world.simulation.statistics.CurrentSimulationStatistics;
import world.simulation.statistics.AveragedSimulationStatistics;
import world.simulation.statistics.TrackedAnimalStatistics;

import java.io.File;
import java.io.FileNotFoundException;
import java.util.ArrayList;

public class Simulation implements IView, IUpdate {

    private final DarwinEvoSimulationEngine engine;
    private final JungleSteppe jungleSteppe;

    private final CurrentSimulationStatistics currentSimulationStatistics;
    private final AveragedSimulationStatistics meanSimulationStatistics;
    private final TrackedAnimalStatistics trackedAnimalStatistics;

    private final JungleSteppeVisualizer mapView;
    private final TextVisualizer trackedAnimalVisualizer;
    private final ArrayList <IView> visualizers;
    private final ArrayList <IUpdate> toUpdate;

    public boolean runnable;

    public Simulation(SimulationParameters parameters, GraphicsContext gc, double positionX, double positionY, double maxSize, double fontSize,
                      String pathToAnimalImage, String pathToGrassImage, String pathToGroundImage, String pathToSpecial)
    {
        this.jungleSteppe = new JungleSteppe(parameters.getWidth(), parameters.getHeight(), parameters.getJungleRatio());
        this.engine = new DarwinEvoSimulationEngine(jungleSteppe, parameters);

        this.currentSimulationStatistics = new CurrentSimulationStatistics();
        this.meanSimulationStatistics = new AveragedSimulationStatistics();
        this.trackedAnimalStatistics = new TrackedAnimalStatistics();

        engine.addNewAnimalObserver(currentSimulationStatistics);
        engine.addNewAnimalObserver(meanSimulationStatistics);
        engine.addNewAnimalObserver(trackedAnimalStatistics);
        jungleSteppe.addGrassNumberObserver(currentSimulationStatistics);
        jungleSteppe.addGrassNumberObserver(meanSimulationStatistics);
        engine.initAnimals();

        this.mapView = new JungleSteppeVisualizer(parameters.getWidth(), parameters.getHeight(), jungleSteppe, gc,
                positionX, positionY, maxSize, pathToAnimalImage, pathToGrassImage, pathToGroundImage,pathToSpecial);

        this.toUpdate = new ArrayList<>();
        toUpdate.add(mapView);
        toUpdate.add(currentSimulationStatistics);
        toUpdate.add(meanSimulationStatistics);
        toUpdate.add(trackedAnimalStatistics);

        this.visualizers = new ArrayList<>();
        visualizers.add(mapView);
        this.trackedAnimalVisualizer = new TextVisualizer(positionX + maxSize/3, positionY + fontSize, maxSize*2/3, maxSize/2.5,
                gc, trackedAnimalStatistics, Color.GRAY);
        visualizers.add(trackedAnimalVisualizer);
        visualizers.add(new TextVisualizer(positionX - maxSize / 3, positionY + fontSize, maxSize*2/3, maxSize/2.5,
                gc, currentSimulationStatistics, Color.SLATEGRAY));

        this.runnable = false;
    }

    @Override
    public void update() {
        if (runnable){
            engine.run();
            for (IUpdate iUpdate : toUpdate)
                iUpdate.update();
        }
    }

    public void renderMap(){
        mapView.update();
        mapView.render();
    }

    @Override
    public void render(){
        if (runnable) {
            if (!trackedAnimalStatistics.isTracked())
                trackedAnimalStatistics.cancelTrackingAnimal();
            for (IView view : visualizers)
                view.render();
        }
    }

    public void clickEvent(double x, double y, GraphicsContext gc){
        if (!runnable){
            Animal animal = mapView.clickedAnimal(x,y);
            if (animal != null){
                trackedAnimalStatistics.setTrackedAnimal(animal);
                trackedAnimalVisualizer.render();
            }
        }
    }

    public void trackAnimal(){
        if (!runnable){
            trackedAnimalStatistics.track();
        }
        else trackedAnimalStatistics.cancelTrackingAnimal();
    }

    public void saveToFile(String filename) {
        try{
            meanSimulationStatistics.writeToFile(filename);
        }
        catch (FileNotFoundException e){
            File file = new File(filename);
            saveToFile(filename);
        }
    }

    public void showAnimalsWithMostCommonGenome(GraphicsContext gc){
        if (!runnable)
            mapView.showTheseAnimals(currentSimulationStatistics.mostCommonGenomeAnimals(), gc);
    }
}
