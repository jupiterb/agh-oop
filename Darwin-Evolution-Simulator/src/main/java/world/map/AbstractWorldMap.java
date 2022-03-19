package world.map;


import world.elements.AbstractWorldMapElement;
import world.elements.Animal;
import world.basic.Vector2d;
import world.elements.Herd;

import java.util.*;

public abstract class AbstractWorldMap implements IWorldMap{

    private final HashMap<Vector2d, Herd> herds;
    protected final HashMap<Vector2d, Integer> freePositions;

    public AbstractWorldMap(){
        this.herds = new HashMap<>();
        this.freePositions = new HashMap<>();
    }

    abstract public Vector2d getLowerLeftCorner();

    abstract public Vector2d getUpperRightCorner();

    @Override
    public boolean place(Animal animal){
        Vector2d position = animal.getPosition();
        herds.putIfAbsent(position, new Herd(position, this));
        if (herds.get(position).addAnimal(animal)){
            freePositions.remove(position);
            return true;
        }
        else return false;
    }

    @Override
    public boolean isOccupied(Vector2d position) {
        return objectAt(position) != null;
    }

    @Override
    public Object objectAt(Vector2d position){
        return herds.get(position);
    }

    @Override
    public boolean remove(AbstractWorldMapElement element){
        if (element instanceof Animal)
            return remove( ((Animal) element).getIndex(), element.getPosition());
        return false;
    }

    public boolean remove(int animalIndex, Vector2d position){
        Herd herd = herds.get(position);
        if (herd != null){
            if (herd.removeAnimal(animalIndex)){
                if (herd.empty()){
                    freePositions.put(herd.getPosition(), 1);
                    herds.remove(position);
                }
                return true;
            }
            else return false;
        }
        else return false;
    }

    @Override
    public boolean canMoveTo(Vector2d position) {
        return position.follows(getLowerLeftCorner()) && position.precedes(getUpperRightCorner());
    }

    public ArrayList<Herd> getHerdArrayList(){
        return new ArrayList<>(herds.values());
    }

    public HashMap<Vector2d, Integer> freePositions(){
        return freePositions;
    }
}
