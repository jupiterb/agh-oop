package world.gui;

import javafx.scene.canvas.GraphicsContext;
import javafx.scene.image.ImageView;
import world.basic.Vector2d;
import world.elements.AbstractWorldMapElement;
import world.elements.Animal;
import world.elements.CellType;
import world.elements.Herd;
import world.map.JungleSteppe;
import world.simulation.IUpdate;

import java.io.File;
import java.util.ArrayList;

public class JungleSteppeVisualizer implements IView, IUpdate
{
    private final double positionX;
    private final double positionY;
    private final double positionSize;
    private final ImageView animal;
    private final ImageView grass;
    private final ImageView ground;
    private final ImageView specialAnimal;

    private final JungleSteppe map;
    private final GraphicsContext graphicsContext;

    private final boolean [][] isEvent;
    private final CellType[][] cellTypes;

    public JungleSteppeVisualizer(int width, int height, JungleSteppe jungleSteppe, GraphicsContext gc,
                                  double positionX, double positionY, double maxSize,
                                  String pathToAnimal, String pathToGrass, String pathToGround, String pathToSpecial) {

        this.positionX = positionX;
        this.positionY = positionY-maxSize;
        this.positionSize = maxSize / Math.max(width, height);

        this.animal = new ImageView(new File(pathToAnimal).toURI().toString());
        animal.setFitWidth(positionSize);
        animal.setFitHeight(positionSize);

        this.grass = new ImageView(new File(pathToGrass).toURI().toString());
        grass.setFitWidth(positionSize);
        grass.setFitHeight(positionSize);

        this.ground = new ImageView(new File(pathToGround).toURI().toString());
        ground.setFitWidth(positionSize);
        ground.setFitHeight(positionSize);

        this.specialAnimal = new ImageView(new File(pathToSpecial).toURI().toString());
        specialAnimal.setFitWidth(positionSize);
        specialAnimal.setFitHeight(positionSize);

        this.map = jungleSteppe;
        this.graphicsContext = gc;

        this.isEvent = new boolean[width][height];
        this.cellTypes = new CellType[width][height];
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                cellTypes[i][j] = CellType.GRASS;
    }

    public void update(){
        for (int i=0; i<= map.getUpperRightCorner().x; i++)
            for (int j=0; j<= map.getUpperRightCorner().y; j++)
            {
                isEvent[i][j] = false;
                CellType currentCellType = map.cellTypeAt(new Vector2d(i,j));
                if (cellTypes[i][j] != currentCellType){
                    cellTypes[i][j] = currentCellType;
                    isEvent[i][j] = true;
                }

            }
    }

    @Override
    public void render() {
        int maxEnergy= 0;
        for (Herd herd : map.getHerdArrayList())
            if (herd.getMaxEnergy() > maxEnergy)
                maxEnergy = herd.getMaxEnergy();

        for (int i=0; i<= map.getUpperRightCorner().x; i++)
            for (int j=0; j<= map.getUpperRightCorner().y; j++) if (isEvent[i][j]){
                switch (cellTypes[i][j]){
                    case ANIMAL -> {
                        graphicsContext.setGlobalAlpha(Math.pow( ( (float)((AbstractWorldMapElement)map.objectAt(new Vector2d(i,j))).getEnergy() / (float)maxEnergy), 0.33 ));
                        graphicsContext.drawImage(animal.getImage(), positionX+i*positionSize, positionY+j*positionSize,
                            positionSize, positionSize);
                        graphicsContext.setGlobalAlpha(1);}
                    case GRASS -> graphicsContext.drawImage(grass.getImage(), positionX+i*positionSize, positionY+j*positionSize,
                            positionSize, positionSize);
                    case GROUND -> graphicsContext.drawImage(ground.getImage(), positionX+i*positionSize, positionY+j*positionSize,
                            positionSize, positionSize);
                }
            }
    }

    public void showTheseAnimals(ArrayList<Animal> animals, GraphicsContext gc){
        for (Animal animal : animals){
            int x = animal.getPosition().x;
            int y = animal.getPosition().y;
            gc.drawImage(specialAnimal.getImage(), positionX+x*positionSize, positionY+y*positionSize,
                    positionSize, positionSize);
        }
    }

    public Animal clickedAnimal(double x, double y){
        if (positionX <= x && x <= positionX+ (map.getUpperRightCorner().x+1)*positionSize
            && positionY <= y && y <= positionY+ (map.getUpperRightCorner().y+1)*positionSize)
        {
            Vector2d position = new Vector2d(
                    (int)((x-positionX) / positionSize),
                    (int)((y-positionY) / positionSize)
            );
            Object object = map.objectAt(position);
            if (object instanceof Herd)
                return ((Herd)object).getMaxEnergyAnimals().get(0);
            else return null;
        }
        else return null;
    }
}
