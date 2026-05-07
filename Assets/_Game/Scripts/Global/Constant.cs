using UnityEngine;

public static class Constant
{
    // tags
    public const string TAG_CHARACTER = "Character";

    // layers
    public const string LAYER_PLAYER = "Player";
    public const string LAYER_ENEMY = "Enemy";
    public const string LAYER_GRAY_BRICK = "GrayBrick";

    // character anim params
    public const string ANIM_IDLE = "Idle";
    public const string ANIM_RUN = "Run";
    public const string ANIM_FALL = "Fall";
    public const string ANIM_WIN = "Win";
    public const string ANIM_LOSE = "Lose";
    // UI anim params
    public const string UI_TRANSITION_ANIM = "Transition";
    public const float UI_TRANSITION_TIME = 1f;

    // pickup brick dimension (length, height, width)
    public const float BRICK_LENGTH = .7f;
    public const float BRICK_HEIGHT = .22f;
    public const float BRICK_WIDTH = .4f;

    // bridge brick dimension (length, height, width)
    public const float BRIDGE_BRICK_LENGTH = 1.4f;
    public const float BRIDGE_BRICK_HEIGHT = .25f;
    public const float BRIDGE_BRICK_WIDTH = .5f;

    // stage dimension
    public const float STAGE_WIDTH = 16f;
    public const float STAGE_LENGTH = 20f;
    public const float STAGE_HEIGHT_DIF = 5f;
    public const float BRIDGE_GAP = 4f;
    public const float BRIDGE_LENGTH = 10f;
    public const float FINISH_BRIDGE_HEIGHT_DIF = 1.5f;
    public const float FINISH_BRIDGE_LENGTH = 3.8f;

    // character params
    public const int CHARACTER_COUNT = 6;
}
