package world.gui;

import javafx.scene.canvas.GraphicsContext;
import javafx.scene.paint.Color;

public class TextVisualizer implements IView{

    private final double positionX;
    private final double positionY;
    private final double width;
    private final double height;
    private final GraphicsContext graphicsContext;
    private final Object object;
    private final Color bgColor;

    public TextVisualizer(double positionX, double positionY, double width, double height,
                          GraphicsContext gc, Object object, Color bgColor)
    {
        this.positionX = positionX;
        this.positionY = positionY;
        this.width = width;
        this.height = height;
        this.graphicsContext = gc;
        this.object = object;
        this.bgColor = bgColor;
    }

    @Override
    public void render() {
        graphicsContext.clearRect(positionX, positionY, width, height);
        graphicsContext.setFill(bgColor);
        graphicsContext.fillRect(positionX, positionY, width, height);
        graphicsContext.setFill(Color.BLACK);
        graphicsContext.fillText(object.toString(), positionX, positionY + graphicsContext.getFont().getSize());
    }
}
