package world.map;

import world.elements.AbstractWorldMapElement;
import world.elements.CellType;
import world.elements.Grass;
import world.observers.IGrassNumberObserver;
import world.basic.Vector2d;

import java.util.*;

public class JungleSteppe extends AbstractWorldMap implements IWorldMap {

    private final Vector2d upperRightCorner;
    static private final Vector2d lowerLeftCorner = new Vector2d(0,0);
    private final Vector2d lowerLeftJungleCorner;
    private final Vector2d upperRightJungleCorner;

    protected final HashMap<Vector2d, Grass> grass;

    private final ArrayList<IGrassNumberObserver> grassNumberObservers;

    private final static Random generator = new Random();

    public JungleSteppe(int width, int height, double jungleRatio){
        super();
        this.upperRightCorner = new Vector2d(width - 1, height - 1);
        this.grass = new HashMap<>();
        int jungleWidth = (int)(width * jungleRatio);
        int jungleHeight = (int)(height * jungleRatio);
        this.lowerLeftJungleCorner = new Vector2d((width - jungleWidth) / 2, (height - jungleHeight) / 2);
        this.upperRightJungleCorner = new Vector2d(this.lowerLeftJungleCorner.x + jungleWidth - 1, this.lowerLeftJungleCorner.y + jungleHeight - 1);
        this.grassNumberObservers = new ArrayList<>();
        for (int i=0; i<=upperRightCorner.x;i++)
            for (int j=0; j<=upperRightCorner.y; j++)
                freePositions.put(new Vector2d(i,j),1);
    }

    public void addGrassNumberObserver(IGrassNumberObserver observer){
        grassNumberObservers.add(observer);
    }

    public void removeGrassNumberObserver(IGrassNumberObserver observer){
        grassNumberObservers.remove(observer);
    }

    public void grassNumberChangeEvent(int difference){
        for (IGrassNumberObserver observer : grassNumberObservers)
            observer.changeGrassNumber(difference);
    }

    public boolean inJungle(Vector2d position){
        return (position.follows(lowerLeftJungleCorner) && position.precedes(upperRightJungleCorner));
    }

    private boolean addGrass(int initialEnergy, Vector2d lowerLeft, Vector2d upperRight, boolean inSteppe){
        for (int i=0; i<10; i++){
            Vector2d position = new Vector2d(generator.nextInt(upperRight.x)+lowerLeft.x, generator.nextInt(upperRight.y)+lowerLeft.y);
            if (!isOccupied(position) && (inJungle(position) ^ inSteppe)){
                Grass grass = new Grass(position, initialEnergy);
                this.grass.put(position, grass);
                grassNumberChangeEvent(1);
                freePositions.remove(position);
                return true;
            }
        }
        ArrayList<Vector2d> potentialPositions = jungleOrSteppeFreePositions(
                new ArrayList<>(freePositions.keySet()), !inSteppe);
        if (potentialPositions.size() > 0){
            Vector2d position = potentialPositions.get(generator.nextInt(potentialPositions.size()));
            Grass grass = new Grass(position, initialEnergy);
            this.grass.put(position, grass);
            grassNumberChangeEvent(1);
            freePositions.remove(position);
            return true;
        }
        else return false;
    }

    private ArrayList<Vector2d> jungleOrSteppeFreePositions(ArrayList<Vector2d> positions, boolean isJungle){
        ArrayList<Vector2d> selectedPositions = new ArrayList<>();
        for (Vector2d position : positions){
            if (inJungle(position) && isJungle)
                selectedPositions.add(position);
            else if (!(inJungle(position) || isJungle))
                selectedPositions.add(position);
        }
        return selectedPositions;
    }

    public boolean addGrassToJungle(int initialEnergy){
        return addGrass(initialEnergy, lowerLeftJungleCorner, upperRightJungleCorner, false);
    }

    public boolean addGrassToSteppe(int initialEnergy){
        return addGrass(initialEnergy, lowerLeftCorner, upperRightCorner, true);
    }

    @Override
    public Object objectAt(Vector2d position) {
        Object result = super.objectAt(position);
        if (result == null)
            result = grass.get(position);
        return result;
    }

    @Override
    public boolean remove(AbstractWorldMapElement element){
        if (!super.remove(element)){
            if (element instanceof Grass && grass.remove(element.getPosition()) != null){
                grassNumberChangeEvent(-1);
                freePositions.put(element.getPosition(), 1);
                return true;
            }
            return false;
        }
        else return true;
    }

    @Override
    public Vector2d getLowerLeftCorner() {
        return lowerLeftCorner;
    }

    @Override
    public Vector2d getUpperRightCorner() {
        return upperRightCorner;
    }

    public Map<Vector2d, Grass> getGrass(){
        return grass;
    }

    @Override
    public boolean canMoveTo(Vector2d position) {
        return true;
    }

    public CellType cellTypeAt(Vector2d position){
        Object object = objectAt(position);
        if (object == null)
            return CellType.GROUND;
        else if (object instanceof Grass)
            return CellType.GRASS;
        else
            return CellType.ANIMAL;
    }
}
