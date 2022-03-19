package world.simulation.statistics;

import world.elements.Genome;
import world.observers.IDeadAnimalObserver;
import world.observers.IEnergyObserver;
import world.observers.IGrassNumberObserver;
import world.observers.INewAnimalObserver;
import world.simulation.IUpdate;

import java.io.FileNotFoundException;
import java.io.PrintWriter;
import java.util.HashMap;
import java.util.Map;

public class AveragedSimulationStatistics extends AbstractSimulationStatistics
        implements INewAnimalObserver, IGrassNumberObserver, IEnergyObserver, IDeadAnimalObserver, IUpdate {

    private double averagedLivingAnimalsNumber;
    private double averagedAnimalsEnergy;
    private double averagedChildrenNumber;
    private double averagedLiveLength;
    private double averagedGrassNumber;

    private final HashMap<Genome, Integer> totalGenomeFrequencyHashMap;
    private Genome mostCommonGenome;

    public AveragedSimulationStatistics(){
        super();
        this.averagedLivingAnimalsNumber = 0;
        this.averagedAnimalsEnergy = 0;
        this.averagedChildrenNumber = 0;
        this.averagedLiveLength = 0;
        this.averagedGrassNumber = 0;
        this.totalGenomeFrequencyHashMap = new HashMap<>();
        mostCommonGenome = null;
    }

    public void update(){
        averagedLivingAnimalsNumber = (averagedLivingAnimalsNumber * epoch + livingAnimalsNumber) / (epoch + 1);
        averagedGrassNumber = (averagedGrassNumber * epoch + grassNumber) / (epoch + 1);
        if (deadAnimalsNumber > 0)
            averagedLiveLength = (averagedLiveLength * epoch + (float)liveLength/(float)deadAnimalsNumber) / (epoch + 1);
        averagedAnimalsEnergy = (averagedAnimalsEnergy * epoch + (float)animalsEnergy / (float)livingAnimalsNumber) / (epoch + 1);
        averagedChildrenNumber = (averagedChildrenNumber * epoch + (float)childrenNumber / (float)livingAnimalsNumber) / (epoch + 1);

        for (Map.Entry<Genome, Integer> entry: genomeFrequencyHashMap.entrySet()){
            if (totalGenomeFrequencyHashMap.get(entry.getKey()) == null)
                totalGenomeFrequencyHashMap.put(entry.getKey(), entry.getValue());
            else{
                int oldFreq = totalGenomeFrequencyHashMap.remove(entry.getKey());
                totalGenomeFrequencyHashMap.put(entry.getKey(), oldFreq + entry.getValue());
            }
            if (mostCommonGenome == null || totalGenomeFrequencyHashMap.get(entry.getKey()) > totalGenomeFrequencyHashMap.get(mostCommonGenome))
                mostCommonGenome = entry.getKey();
        }

        super.update();
    }

    @Override
    protected float livingAnimalsNumber() {
        return (float) averagedLivingAnimalsNumber;
    }

    @Override
    protected float grassNumber() {
        return (float) averagedGrassNumber;
    }

    @Override
    protected float meanLiveLength() {
        return (float) averagedLiveLength;
    }

    @Override
    protected float meanEnergy() {
        return (float) averagedAnimalsEnergy;
    }

    @Override
    protected float meanChildrenNumber() {
        return (float) averagedChildrenNumber;
    }

    @Override
    protected Genome getMostCommonGenome() {
        return mostCommonGenome;
    }

    @Override
    public String toString(){
        String stats = super.toString();
        return stats + " ( " + ((float)totalGenomeFrequencyHashMap.get(mostCommonGenome) / (float)epoch) + " ) ";
    }

    public void writeToFile(String filename) throws FileNotFoundException {
        PrintWriter writer = new PrintWriter(filename);
        writer.println(this.toString());
        writer.close();
    }
}
