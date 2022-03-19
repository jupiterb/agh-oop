package world.simulation.statistics;

import world.elements.Animal;
import world.observers.IDeadAnimalObserver;
import world.observers.INewAnimalObserver;
import world.simulation.IUpdate;

import java.util.ArrayList;

public class TrackedAnimalStatistics implements INewAnimalObserver, IDeadAnimalObserver, IUpdate {

    private Animal trackedAnimal;
    private final ArrayList <Animal> children;
    private final ArrayList <Animal> otherDescendants;
    private boolean isDead;
    private boolean tracked;
    private int startEpoch;
    private int startTrackedAnimalAge;
    private int epoch;

    public TrackedAnimalStatistics(){
        this.trackedAnimal = null;
        this.children = new ArrayList<>();
        this.otherDescendants = new ArrayList<>();
        epoch = 0;
    }

    public void update(){
        epoch++;
    }

    @Override
    public void deadAnimalEvent(Animal animal) {
        if (animal == trackedAnimal)
            isDead = true;
    }

    @Override
    public void newAnimalEvent(Animal animal, boolean isChild) {
        if (isChild){
            if (isChild(animal)) children.add(animal);
            else if (isOtherDescendant(animal)) otherDescendants.add(animal);
        }
    }

    private boolean isChild(Animal animal){
        for (Animal parent : animal.getParents()) if (parent == trackedAnimal) return true;
        return false;
    }

    private boolean isOtherDescendant(Animal animal){
        for (Animal parent : animal.getParents()){
            for (Animal child : children) if (parent == child) return true;
            for (Animal descendant : otherDescendants) if (parent == descendant) return true;
        }
        return false;
    }

    public void setTrackedAnimal(Animal animal){
        cancelTrackingAnimal();
        trackedAnimal = animal;
        isDead = false;
        animal.addDeadAnimalObserver(this);
        this.startEpoch = epoch;
        startTrackedAnimalAge = animal.getAge();
    }

    public void track(){
        if (trackedAnimal!=null)
            tracked = true;
    }

    public void cancelTrackingAnimal(){
        trackedAnimal = null;
        tracked = false;
        children.clear();
        otherDescendants.clear();
    }

    public int getChildrenNumber(){
        return children.size();
    }

    public int getOtherDescendantsNumber(){
        return otherDescendants.size();
    }

    public String toString(){
        String status = "";
        if (trackedAnimal != null){
            status += ("This Animal genome :\n" + trackedAnimal.getGenome() + "\n");
            if (tracked){
                status += ( "Epoch of tracking this Animal : " + (epoch - startEpoch) +
                        "\nChildren number : " + getChildrenNumber() +
                        "\nOther Descendants number : " + getOtherDescendantsNumber() );
                if (isDead)
                    status += ("\nTracked Animal dead at " +
                            ((Integer)(startEpoch + trackedAnimal.getAge() - startTrackedAnimalAge)).toString() +
                            " epoch\n at the age of " + ((Integer)trackedAnimal.getAge()).toString() );
                else
                    status += ( "\nTracked Animal lives and has " + ((Integer)trackedAnimal.getAge()).toString() );
            }
        }
        else{
            status += """
                    To track animal :
                    1. Pause the simulation
                    2. Click animal on the map
                    3. Click button 'Track Animal'
                    4. Run the simulation""";
        }
        return status;
    }

    public boolean isTracked(){
        return tracked;
    }
}
