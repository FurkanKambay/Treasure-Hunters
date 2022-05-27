namespace Game.Helpers
{
    public enum CellType
    {
        Unrevealed,
        Plain,
        Obstacle,
        Treasure,
        Artifact,
        Wheel
    }

    public enum HexDirection
    {
        UpRight,
        Right,
        DownRight,
        DownLeft,
        Left,
        UpLeft
    }

    public enum WheelResult
    {
        GainTreasure,
        LoseTreasure,
        GainArtifact,
        LoseArtifact
    }

    public enum CellState
    {
        Hidden,
        RevealedDisabled,
    }
}
