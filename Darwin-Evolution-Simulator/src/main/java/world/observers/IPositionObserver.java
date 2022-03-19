package world.observers;

import world.basic.Vector2d;

public interface IPositionObserver {

    void positionChanged(Vector2d oldPosition, Vector2d newPosition, int index);
}
