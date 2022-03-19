package world.simulation.engine;

import world.basic.Vector2d;
import world.elements.Animal;
import world.elements.Genome;
import world.elements.Grass;
import world.elements.Herd;
import world.map.JungleSteppe;
import world.observers.INewAnimalObserver;
import world.simulation.SimulationParameters;

import java.util.*;

public class DarwinEvoSimulationEngine  implements IEngine {

    private final JungleSteppe map;
    private final SimulationParameters parameters;
    private final ArrayList<INewAnimalObserver> newAnimalObservers;

    private final static Random generator = new Random();

    public DarwinEvoSimulationEngine(JungleSteppe map, SimulationParameters parameters) {
        this.map = map;
        this.parameters = parameters;
        this.newAnimalObservers = new ArrayList<>();
    }

    public void addNewAnimalObserver(INewAnimalObserver observer){
        newAnimalObservers.add(observer);
    }

    public void removeNewAnimalObserver(INewAnimalObserver observer){
        newAnimalObservers.remove(observer);
    }

    public void newAnimalEvent(Animal animal, boolean isChild) {
        for (INewAnimalObserver observer : newAnimalObservers)
            observer.newAnimalEvent(animal, isChild);
    }

    public void initAnimals(){
        int startFrequency = parameters.getStartAnimalsAmount();
        int startEnergy = parameters.getStartEnergy();
        Vector2d position = null;
        for (int i=0; i<startFrequency; i++){
            for (int j=0; j<10; j++){
                position = new Vector2d(generator.nextInt(map.getUpperRightCorner().x), generator.nextInt(map.getUpperRightCorner().y));
                if (!map.isOccupied(position))
                    break;
            }
            if (map.isOccupied(position)){
                ArrayList<Vector2d> positions = new ArrayList<>(map.freePositions().keySet());
                int positionIndex = generator.nextInt(positions.size());
                position = positions.get(positionIndex);
            }
            Animal animal = new Animal(position, startEnergy, new Genome(32), map );
            map.place(animal);
            newAnimalEvent(animal, false);
        }
    }

    private void move() {
        for (Herd herd : map.getHerdArrayList())
            herd.move();
    }

    private void foodAnimals(){
        ArrayList<Grass> grasses = new ArrayList<>(map.getGrass().values());
        for (Grass grass : grasses){
            Object herd = map.objectAt(grass.getPosition());
            if (herd instanceof Herd){
                ((Herd)herd).food(grass.getEnergy());
                map.remove(grass);
            }
        }
    }

    private void reproduceAnimals(){
        ArrayList<Animal> children = new ArrayList<>();
        for (Herd herd : map.getHerdArrayList()){
            Animal child = herd.reproduce(null, 4, parameters.getStartEnergy() / 2);
            if (child != null){
                newAnimalEvent(child, true);
                children.add(child);
            }
        }
        for (Animal child : children)
            map.place(child);
    }

    private void addGrass(){
        map.addGrassToSteppe(parameters.getPlantEnergy());
        map.addGrassToJungle(parameters.getPlantEnergy());
    }

    private void ageAnimals(){
        for (Herd herd : map.getHerdArrayList())
            herd.aged(parameters.getMoveEnergy());
    }

    @Override
    public void run(){
        move();
        foodAnimals();
        reproduceAnimals();
        addGrass();
        ageAnimals();
    }
}
