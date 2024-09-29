using EaseType = Godot.Tween.EaseType;
using Godot;
using TransType = Godot.Tween.TransitionType;

namespace GodotUtils;

/*
 * Create a path from a set of points with options to add curvature and
 * animate the attached sprite.
 */
public partial class GPath : Path2D
{
    public bool Rotates
    {
        get => _pathFollow.Rotates;
        set => _pathFollow.Rotates = value;
    }

    private PathFollow2D _pathFollow;
    private Vector2[] _points;
    private Sprite2D _sprite;
    private GTween _tween;
    private float[] _tweenValues;
    private int _tweenIndex;
    private TransType _transType = TransType.Sine;
    private EaseType _easeType = EaseType.Out;
    private double _animSpeed;
    private Color _color;
    private float _width;
    private int _dashes;

    public GPath(Vector2[] points, Color color, int width = 5, int dashes = 0, double animSpeed = 1)
    {
        _points = points;
        Curve = new Curve2D();
        _pathFollow = new PathFollow2D { Rotates = false };
        _tween = new GTween(_pathFollow);
        AddChild(_pathFollow);

        _color = color;
        _width = width;
        _dashes = dashes;
        _animSpeed = animSpeed;

        // Add points to the path
        for (int i = 0; i < points.Length; i++)
        {
            Curve.AddPoint(points[i]);
        }

        CalculateTweenValues();
    }

    public override void _Draw()
    {
        Vector2[] points = Curve.GetBakedPoints();

        for (int i = 0; i < points.Length - 1; i += (_dashes + 1))
        {
            Vector2 A = points[i];
            Vector2 B = points[i + 1];

            DrawLine(A, B, _color, _width, true);
        }
    }

    public void SetLevelProgress(int v)
    {
        _pathFollow.Progress = _tweenValues[v - 1];
    }

    public void AnimateTo(int targetIndex)
    {
        if (targetIndex > _tweenIndex)
        {
            AnimateForwards(targetIndex - _tweenIndex);
        }
        else
        {
            AnimateBackwards(_tweenIndex - targetIndex);
        }
    }

    public int AnimateForwards(int step = 1)
    {
        _tweenIndex = Mathf.Min(_tweenIndex + step, _tweenValues.Length - 1);
        Animate(true);
        return _tweenIndex;
    }

    public int AnimateBackwards(int step = 1)
    {
        _tweenIndex = Mathf.Max(_tweenIndex - step, 0);
        Animate(false);
        return _tweenIndex;
    }

    public void AddSprite(Sprite2D sprite)
    {
        _sprite = sprite;
        _pathFollow.AddChild(sprite);
    }

    /// <summary>
    /// Add curves to the path. The curve distance is how far each curve is pushed
    /// out.
    /// </summary>
    public void AddCurves(int curveSize = 50, int curveDistance = 50)
    {
        // Add aditional points to make each line be curved
        int invert = 1;

        for (int i = 0; i < _points.Length - 1; i++)
        {
            Vector2 A = _points[i];
            Vector2 B = _points[i + 1];

            Vector2 center = (A + B) / 2;
            Vector2 offset = ((B - A).Orthogonal().Normalized() * curveDistance * invert);
            Vector2 newPos = center + offset;

            // Switch between sides so curves flow more naturally
            invert *= -1;

            Vector4 v;

            // These values were found through trial and error
            // If you see a simpler pattern than this, please tell me lol
            if (B.Y >= A.Y)
            {
                if (B.X >= A.X)
                {
                    // Next point is under and after first point
                    v = new Vector4(-1, -1, 1, 1);
                }
                else
                {
                    // Next point is under and before first point
                    v = new Vector4(1, -1, -1, 1);
                }
            }
            else
                if (B.X <= A.X)
            {
                // Next point is over and before first point
                v = new Vector4(1, 1, -1, -1);
            }
            else
            {
                // Next point is over and after first point
                v = new Vector4(-1, 1, 1, -1);
            }

            int index = 1 + i * 2;

            // Insert the curved point at the index in the curve
            Curve.AddPoint(newPos,
                new Vector2(v.X, v.Y) * curveSize,
                new Vector2(v.Z, v.W) * curveSize, index);
        }

        // Since new points were added, the tween values need to be re-calulcated
        CalculateTweenValues();
    }

    private void CalculateTweenValues()
    {
        _tweenValues = new float[_points.Length];
        for (int i = 0; i < _points.Length; i++)
        {
            _tweenValues[i] = Curve.GetClosestOffset(_points[i]);
        }
    }

    private void Animate(bool forwards)
    {
        _tween = new(this);
        _tween.Animate(PathFollow2D.PropertyName.Progress, _tweenValues[_tweenIndex],
            CalculateDuration(forwards)).SetTrans(_transType).SetEase(_easeType);
    }

    private double CalculateDuration(bool forwards)
    {
        // The remaining distance left to go from the current sprites progress
        float remainingDistance = Mathf.Abs(
            _tweenValues[_tweenIndex] - _pathFollow.Progress);

        int startIndex = 0;

        // Dynamically calculate the start index
        for (int i = 0; i < _tweenValues.Length; i++)
        {
            if (_pathFollow.Progress <= _tweenValues[i])
            {
                startIndex = i;
                break;
            }
        }

        // The number of level icons left to pass
        int levelIconsLeft = Mathf.Max(1, Mathf.Abs(_tweenIndex - startIndex));

        double duration = remainingDistance / 150 / _animSpeed / levelIconsLeft;

        return duration;
    }
}

