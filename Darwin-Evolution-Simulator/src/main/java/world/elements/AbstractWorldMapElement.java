package world.elements;

import world.basic.Vector2d;

public abstract class AbstractWorldMapElement{

    protected Vector2d position;

    protected int energy;

    public AbstractWorldMapElement(Vector2d initialPosition, int initialEnergy){
        this.position = initialPosition;
        this.energy = initialEnergy;
    }

    public Vector2d getPosition(){
        return this.position;
    }

    public int getEnergy(){
        return this.energy;
    }
}
