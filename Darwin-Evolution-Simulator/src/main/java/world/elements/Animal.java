package world.elements;

import world.observers.IDeadAnimalObserver;
import world.observers.IEnergyObserver;
import world.observers.IPositionObserver;
import world.basic.MapDirection;
import world.basic.Vector2d;
import world.map.AbstractWorldMap;

import java.util.ArrayList;
import java.util.Random;

public class Animal extends AbstractWorldMapElement {

    private int index;
    private int age;
    private final Genome genome;
    private MapDirection dir;

    private int childrenNumber;
    private Animal [] parents;

    private final AbstractWorldMap map;

    private final ArrayList<IPositionObserver> positionObservers;
    private final ArrayList<IEnergyObserver> energyObservers;
    private final ArrayList<IDeadAnimalObserver> deadAnimalObservers;

    private final static Random generator = new Random();


    public Animal(Vector2d initialPosition, int initialEnergy, Genome genome, AbstractWorldMap map){
        super(initialPosition, initialEnergy);
        this.index = 0;
        this.age = 0;
        this.childrenNumber = 0;
        this.parents = new Animal[0];
        this.genome = genome;
        this.dir = MapDirection.NORTH;
        this.map = map;
        this.positionObservers= new ArrayList<>();
        this.energyObservers = new ArrayList<>();
        this.deadAnimalObservers = new ArrayList<>();
    }

    public Animal(Vector2d initialPosition, int initialEnergy, Genome genome, AbstractWorldMap map, Animal [] parents){
        this(initialPosition, initialEnergy, genome, map);
        this.parents = parents;
    }

    public String toString(){
        return (Integer.valueOf(energy)).toString();
    }

    public void move(){
        short [] dna = genome.getDna();
        int movement = dna[generator.nextInt(dna.length)];
        this.dir = dir.rotate(movement);
        Vector2d oldPosition = new Vector2d(getPosition().x, getPosition().y);
        position = position.add(dir.toUnitVector());

        if (position.x > map.getUpperRightCorner().x)
            position = position.subtract(new Vector2d(map.getUpperRightCorner().x+1, 0));
        if (position.y > map.getUpperRightCorner().y)
            position = position.subtract(new Vector2d(0, map.getUpperRightCorner().y+1));

        if (position.x < map.getLowerLeftCorner().x)
            position = position.add(new Vector2d(map.getUpperRightCorner().x+1, 0));
        if (position.y < map.getLowerLeftCorner().y)
            position = position.add(new Vector2d(0, map.getUpperRightCorner().y+1));

        positionChanged(oldPosition);
    }

    public void food(int foodEnergy) {
        energy += foodEnergy;
        energyChanged(foodEnergy);
    }

    public void raiseChild(Animal child, int energyPartForChild) {
        childrenNumber ++ ;
        int energyForReproduce = energy / energyPartForChild;
        child.food(energyForReproduce);
        energy -= energyForReproduce;
        energyChanged(-energyForReproduce);
    }

    public void aged(int moveEnergy) {
        age ++;
        energy -= moveEnergy;
        if (energy <= 0){
            energyChanged(-moveEnergy-energy);
            deadEvent();
        }
        energyChanged(-moveEnergy);
    }

    public void addPositionObserver(IPositionObserver observer){
        positionObservers.add(observer);
    }

    public void addEnergyObserver(IEnergyObserver observer){
        energyObservers.add(observer);
    }

    public void addDeadAnimalObserver(IDeadAnimalObserver observer) {
        deadAnimalObservers.add(observer);
    }

    public void removePositionObserver(IPositionObserver observer){
        positionObservers.remove(observer);
    }

    public void removeEnergyObserver(IEnergyObserver observer){
        energyObservers.remove(observer);
    }

    public void removeDeadAnimalObserver(IDeadAnimalObserver observer){
        deadAnimalObservers.remove(observer);
    }

    protected void positionChanged(Vector2d oldPosition){
        for (int i=0; i<positionObservers.size(); i++)
            positionObservers.get(i).positionChanged(oldPosition, position, index);
    }

    protected void energyChanged(int difference){
        for (int i=0; i<energyObservers.size(); i++)
            energyObservers.get(i).energyChanged(energy, difference, index);
    }

    protected void deadEvent(){
        for (int i=0; i<deadAnimalObservers.size(); i++)
            deadAnimalObservers.get(i).deadAnimalEvent(this);
    }

    protected void setIndex(int newIndex){
        index = newIndex;
    }

    public int getIndex(){
        return index;
    }

    public int getAge(){
        return age;
    }

    public int getEnergy(){
        return energy;
    }

    public Genome getGenome() {
        return genome;
    }

    public int getChildrenNumber(){
        return childrenNumber;
    }

    public Animal[] getParents(){
        return parents;
    }
}