package world.elements;

import world.map.AbstractWorldMap;
import world.observers.IDeadAnimalObserver;
import world.observers.IEnergyObserver;
import world.observers.IPositionObserver;
import world.basic.MapDirection;
import world.basic.Vector2d;

import java.util.*;

public class Herd extends AbstractWorldMapElement implements IPositionObserver, IEnergyObserver, IDeadAnimalObserver {


    private static class AnimalsComparator implements Comparator<Animal> {

        @Override
        public int compare(Animal o1, Animal o2) {
            if (o2.getEnergy() != o1.getEnergy())
                return o2.getEnergy() - o1.getEnergy();
            else return o2.getIndex() - o1.getIndex();
        }
    }

    private static final AnimalsComparator comparator = new AnimalsComparator();
    private final static Random generator = new Random();

    protected final SortedSet <Animal> sortedAnimals;
    private final AbstractWorldMap map;
    private int lastAssignedIndex=0;

    public Herd(Vector2d initialPosition, AbstractWorldMap map) {
        super(initialPosition, 0);
        this.sortedAnimals = new TreeSet<>(comparator);
        this.map = map;
    }

    @Override
    public String toString(){
        return String.valueOf(getMaxEnergy());
    }

    public boolean addAnimal(Animal animal){
        if (animal != null && animal.getPosition().equals(position)){
            animal.setIndex(lastAssignedIndex);
            lastAssignedIndex ++;
            if (sortedAnimals.add(animal)){
                animal.addEnergyObserver(this);
                animal.addPositionObserver(this);
                animal.addDeadAnimalObserver(this);
                return true;
            }
            else return false;
        }
        else return false;
    }

    public boolean removeAnimal(int animalIndex){
        Animal animal = getAnimal(animalIndex);
        if (animal != null){
            animal.removeEnergyObserver(this);
            animal.removePositionObserver(this);
            animal.removeDeadAnimalObserver(this);
            sortedAnimals.removeIf(a -> a.getIndex() == animalIndex);
            return true;
        }
        return false;
    }

    private Animal getAnimal(int animalIndex){
        for (Animal animal : sortedAnimals) {
            if (animal.getIndex() == animalIndex)
                return animal;
        }
        return null;
    }

    @Override
    public void positionChanged(Vector2d oldPosition, Vector2d newPosition, int index) {
        Animal animal = getAnimal(index);
        if (!map.remove(index, oldPosition))
            removeAnimal(index); //case if herd is not on the map
        if (animal.getEnergy() > 0)
            map.place(animal);
    }

    @Override
    public void deadAnimalEvent(Animal animal) {
        if (energy <= 0){
            if (!map.remove(animal))
                removeAnimal(animal.getIndex());
        }
    }

    @Override
    public void energyChanged(int newEnergy, int energyDifference, int index) {
        if (newEnergy > 0){
            Animal toUpdate = null;
            Animal previous = null;
            for (Animal animal : sortedAnimals){
                if (previous != null && comparator.compare(previous, animal) > 0){
                    if (previous.getIndex() == index){
                        toUpdate = previous;
                        break;
                    }
                    else if (animal.getIndex() == index){
                        toUpdate = animal;
                        break;
                    }
                }
                previous = animal;
            }
            if (toUpdate != null){
                removeAnimal(index);
                addAnimal(toUpdate);
            }
        }
    }

    public int getMaxEnergy(){
        if (sortedAnimals.size() > 0) return sortedAnimals.first().getEnergy();
        else return 0;
    }

    @Override
    public int getEnergy() {
        return getMaxEnergy();
    }

    public ArrayList<Animal> getMaxEnergyAnimals(){
        ArrayList<Animal> maxEnergyAnimals = new ArrayList<>();
        for (Animal animal : sortedAnimals){
            if (animal.getEnergy() == getMaxEnergy()) maxEnergyAnimals.add(animal);
            else break;
        }
        return maxEnergyAnimals;
    }

    private ArrayList<Animal> getSecondMaxEnergyAnimals(){
        ArrayList<Animal> secondMaxEnergyAnimals = new ArrayList<>();
        int secondMaxEnergy = 0;
        for (Animal animal : sortedAnimals){
            if (animal.getEnergy() < getMaxEnergy() && animal.getEnergy() >= secondMaxEnergy){
                secondMaxEnergyAnimals.add(animal);
                secondMaxEnergy = animal.getEnergy();
            }
            else if (animal.getEnergy() < secondMaxEnergy) break;
        }
        return secondMaxEnergyAnimals;
    }

    private Animal [] selectParents(){
        Animal [] parents = new Animal[2];
        ArrayList<Animal> potentialParents = getMaxEnergyAnimals();
        if (potentialParents.size() == 1){
            parents[0] = potentialParents.get(0);
            potentialParents = getSecondMaxEnergyAnimals();
            parents[1] = potentialParents.get(generator.nextInt(potentialParents.size()));
        }
        else{
            parents[0] = potentialParents.get(generator.nextInt(potentialParents.size()));
            parents[1] = potentialParents.get(generator.nextInt(potentialParents.size()));
            while (parents[0].getIndex() == parents[1].getIndex())
                parents[1] = potentialParents.get(generator.nextInt(potentialParents.size()));
        }
        return parents;
    }

    private Vector2d selectChildPosition(){
        Vector2d potentialPosition;
        int hm=0; // huw much
        Vector2d [] potentialPositions = new Vector2d[MapDirection.values().length];
        for (MapDirection direction : MapDirection.values()){
            potentialPosition = position.add(direction.toUnitVector());
            if (!map.isOccupied(potentialPosition)){
                potentialPositions[hm] = potentialPosition;
                hm++;
            }
        }
        if (hm>0) return potentialPositions[generator.nextInt(hm)];
        else return position.add(MapDirection.values()[generator.nextInt(MapDirection.values().length)].toUnitVector());
    }

    public Animal reproduce(Animal child, int energyPartForChild, int requiredEnergy){
        if (sortedAnimals.size() >= 2 && getMaxEnergy() >= Math.max(energyPartForChild, requiredEnergy)){
            Animal [] parents = selectParents();
            for (Animal parent : parents)
                if (parent.getEnergy() < requiredEnergy) return child;
            child = new Animal(selectChildPosition(), 0,
                                new Genome(32, parents[0].getGenome(), parents[1].getGenome()), map, parents);
            for (Animal parent : parents)
                parent.raiseChild(child, energyPartForChild);
        }
        return child;
    }

    public void aged(int moveEnergy) {
        ArrayList<Animal> animals = new ArrayList<>(sortedAnimals);
        int s = animals.size();
        for (int i=0; i<s; i++)
            animals.get(i).aged(moveEnergy);
    }

    public void move() {
        ArrayList<Animal> animals = new ArrayList<>(sortedAnimals);
        for (int i=0; i<animals.size(); i++)
            animals.get(i).move();
    }

    public void food(int foodEnergy) {
        ArrayList<Animal> animalsEat = getMaxEnergyAnimals();
        for (Animal animal : animalsEat)
            animal.food(foodEnergy / animalsEat.size());
    }

    public boolean empty(){
        return sortedAnimals.size() == 0;
    }
}
