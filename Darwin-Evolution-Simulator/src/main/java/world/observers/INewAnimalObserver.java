package world.observers;

import world.elements.Animal;

public interface INewAnimalObserver {

    void newAnimalEvent(Animal animal, boolean isChild);
}
