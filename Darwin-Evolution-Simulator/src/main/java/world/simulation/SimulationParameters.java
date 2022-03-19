package world.simulation;

import org.json.*;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;


public class SimulationParameters{

    private final int width;
    private final int height;
    private final int startEnergy;
    private final int moveEnergy;
    private final int plantEnergy;
    private final double jungleRatio;
    private final int startAnimalsAmount;
    private final int interval;

    public SimulationParameters(String filename) throws IllegalArgumentException{
        String content = null;
        try{
            content = new String((Files.readAllBytes(Paths.get(filename))));
        } catch (IOException e){
            e.printStackTrace();
        }
        assert content != null;
        JSONObject object = new JSONObject(content);
        this.width = object.getInt("width");
        this.height = object.getInt("height");
        this.startEnergy = object.getInt("startEnergy");
        if (startEnergy < 1) throw new IllegalArgumentException("\nStart energy should be" +
                                                                "\npositive integer");
        this.moveEnergy = object.getInt("moveEnergy");
        if (moveEnergy < 1) throw new IllegalArgumentException("\nMove energy should be" +
                                                                "\npositive integer");
        this.plantEnergy = object.getInt("plantEnergy");
        if (plantEnergy < 1) throw new IllegalArgumentException("\nPlant energy should be" +
                                                                "\npositive integer");
        this.jungleRatio  =object.getDouble("jungleRatio");
        if (jungleRatio < 0 || jungleRatio > 1) throw new IllegalArgumentException("\nJungle ratio should be" +
                "                                               \nnumber between 0 and 1");
        this.startAnimalsAmount = object.getInt("startAnimalsAmount");
        if (startAnimalsAmount > width * height) throw new IllegalArgumentException("\nThere cannot be more animals" +
                                                                "\nthan number of positions on" +
                                                                "\nthe map");
        this.interval = object.getInt("interval");

    }

    public int getWidth(){
        return width;
    }

    public int getHeight(){
        return height;
    }

    public int getStartEnergy(){
        return startEnergy;
    }

    public int getMoveEnergy(){
        return moveEnergy;
    }

    public int getPlantEnergy(){
        return plantEnergy;
    }

    public double getJungleRatio() {
        return jungleRatio;
    }

    public int getStartAnimalsAmount() {
        return startAnimalsAmount;
    }

    public int getInterval(){
        return interval;
    }

    @Override
    public String toString(){
        return "\n\n\nMap size : " + width + " x " + height +
                "\n\n\nJungle ratio : " + jungleRatio +
                "\n\n\nStart energy : " + startEnergy +
                "\n\n\nGrass energy : " + plantEnergy +
                "\n\n\nMove energy : " + moveEnergy +
                "\n\n\nStart animals number :" + startAnimalsAmount +
                "\n\n\nTime interval :" + interval;
    }
}
