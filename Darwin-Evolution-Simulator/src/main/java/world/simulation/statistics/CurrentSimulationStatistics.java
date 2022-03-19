package world.simulation.statistics;


import world.elements.Animal;
import world.elements.Genome;
import world.observers.IDeadAnimalObserver;
import world.observers.IEnergyObserver;
import world.observers.IGrassNumberObserver;
import world.observers.INewAnimalObserver;
import world.simulation.IUpdate;

import java.util.ArrayList;
import java.util.HashMap;

public class CurrentSimulationStatistics extends AbstractSimulationStatistics
        implements INewAnimalObserver, IGrassNumberObserver, IEnergyObserver, IDeadAnimalObserver, IUpdate {

    private final HashMap<Genome, ArrayList<Animal>> animalsGenomeMap;
    private Genome mostCommonGenome;

    public CurrentSimulationStatistics(){
        super();
        this.animalsGenomeMap = new HashMap<>();
        mostCommonGenome = null;
    }

    @Override
    public void newAnimalEvent(Animal animal, boolean isChild){
        super.newAnimalEvent(animal, isChild);
        if (animal != null){
            Genome genome = animal.getGenome();

            animalsGenomeMap.putIfAbsent(genome, new ArrayList<>());
            animalsGenomeMap.get(genome).add(animal);

            int currentFreq = genomeFrequencyHashMap.get(genome);
            if (mostCommonGenome == null || currentFreq >= genomeFrequencyHashMap.get(getMostCommonGenome()))
                mostCommonGenome = genome;
        }
    }

    @Override
    public void deadAnimalEvent(Animal animal) {
        super.deadAnimalEvent(animal);

        Genome genome = animal.getGenome();

        if (genome.equals(mostCommonGenome)) {
            for (Genome g : genomeFrequencyHashMap.keySet())
                if (mostCommonGenome == null || genomeFrequencyHashMap.get(g) > genomeFrequencyHashMap.get(mostCommonGenome))
                    mostCommonGenome = g;
        }

        animalsGenomeMap.get(genome).remove(animal);
        if (animalsGenomeMap.get(genome).isEmpty())
            animalsGenomeMap.remove(genome);
    }

    @Override
    protected float livingAnimalsNumber(){
        return livingAnimalsNumber;
    }

    @Override
    protected float grassNumber() {
        return grassNumber;
    }

    @Override
    protected float meanLiveLength(){
        return (float)liveLength / (float)deadAnimalsNumber;
    }

    @Override
    protected float meanEnergy(){
        return (float)animalsEnergy / (float)livingAnimalsNumber;
    }

    @Override
    protected float meanChildrenNumber(){
        return (float)childrenNumber / (float)livingAnimalsNumber;
    }

    @Override
    protected Genome getMostCommonGenome() {
        return mostCommonGenome;
    }

    @Override
    public String toString(){
        String stats = super.toString();
        return stats + " ( " + genomeFrequencyHashMap.get(mostCommonGenome) + " ) ";
    }

    public ArrayList<Animal> mostCommonGenomeAnimals(){
        if (mostCommonGenome != null)
            return animalsGenomeMap.get(mostCommonGenome);
        else
            return null;
    }
}
