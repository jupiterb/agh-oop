package world.simulation.statistics;

import world.elements.Animal;
import world.elements.Genome;
import world.observers.IDeadAnimalObserver;
import world.observers.IEnergyObserver;
import world.observers.IGrassNumberObserver;
import world.observers.INewAnimalObserver;
import world.simulation.IUpdate;

import java.util.*;

abstract public class AbstractSimulationStatistics
        implements INewAnimalObserver, IGrassNumberObserver, IEnergyObserver, IDeadAnimalObserver, IUpdate {

    protected int livingAnimalsNumber;
    protected int animalsEnergy;
    protected int childrenNumber;
    protected int deadAnimalsNumber;
    protected int liveLength;
    protected int grassNumber;

    protected final HashMap<Genome, Integer> genomeFrequencyHashMap;

    protected int epoch;

    public AbstractSimulationStatistics(){
        this.livingAnimalsNumber = 0;
        this.deadAnimalsNumber = 0;
        this.grassNumber = 0;
        this.animalsEnergy = 0;
        this.liveLength = 0;
        this.childrenNumber = 0;
        this.epoch = 0;
        this.genomeFrequencyHashMap = new HashMap<>();
    }

    @Override
    public void changeGrassNumber(int difference) {
        grassNumber += difference;
    }

    @Override
    public void newAnimalEvent(Animal animal, boolean isChild) {
        if (animal != null)
        {
            animal.addEnergyObserver(this);
            animal.addDeadAnimalObserver(this);
            animalsEnergy += animal.getEnergy();
            livingAnimalsNumber ++;
            if (isChild)
                childrenNumber += animal.getParents().length;

            Genome genome = animal.getGenome();
            int currentFreq = genomeFrequencyHashMap.getOrDefault(genome, 0) + 1;
            genomeFrequencyHashMap.put(genome, currentFreq);
        }
    }

    @Override
    public void energyChanged(int newEnergy, int energyDifference, int index) {
        animalsEnergy += energyDifference;
    }

    @Override
    public void deadAnimalEvent(Animal animal) {
        liveLength += animal.getAge();
        deadAnimalsNumber ++;
        livingAnimalsNumber --;
        childrenNumber -= animal.getChildrenNumber();

        Genome genome = animal.getGenome();
        int currentFreq = genomeFrequencyHashMap.get(genome) - 1;
        if (currentFreq == 0)
            genomeFrequencyHashMap.remove(genome);
        else
            genomeFrequencyHashMap.put(genome, currentFreq);
    }

    public void update(){
        epoch ++;
    }

    abstract protected float livingAnimalsNumber();

    abstract protected float grassNumber();

    abstract protected float meanLiveLength();

    abstract protected float meanEnergy();

    abstract protected float meanChildrenNumber();

    abstract protected Genome getMostCommonGenome();

    @Override
    public String toString(){
        String stats = "Epoch : " + epoch +
                "\nLiving animals number : " + livingAnimalsNumber() +
                "\nGrass number : " + grassNumber() +
                "\nMean live length : " + meanLiveLength() +
                "\nMean energy per animal : " + meanEnergy() +
                "\nMean children number per animal : " + meanChildrenNumber();
        assert getMostCommonGenome() != null;
        stats += "\nMost common genome : \n" + getMostCommonGenome();
        return stats;
    }
}
