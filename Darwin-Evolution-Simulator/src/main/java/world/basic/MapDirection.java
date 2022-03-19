package world.basic;

public enum MapDirection{
    NORTH,
    NORTH_WEST,
    WEST,
    SOUTH_WEST,
    SOUTH,
    SOUTH_EAST,
    EAST,
    NORTH_EST;

    public MapDirection rotate(int moveDirection){
        return MapDirection.values()[(this.ordinal() + moveDirection) % 8];
    }

    public Vector2d toUnitVector(){
        return switch (this){
            case NORTH -> new Vector2d(0, 1);
            case NORTH_WEST -> new Vector2d(1, 1);
            case WEST -> new Vector2d(1, 0);
            case SOUTH_WEST -> new Vector2d(1, -1);
            case SOUTH -> new Vector2d(0, -1);
            case SOUTH_EAST -> new Vector2d(-1, -1);
            case EAST -> new Vector2d(-1, 0);
            case NORTH_EST -> new Vector2d(-1, 1);
        };
    }
}

