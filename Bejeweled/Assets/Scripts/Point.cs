using System;

public class Point : IEquatable<Point> {
  public int x;
  public int y;

  public Point() : this(0, 0) {
  }

  public Point(int x, int y) {
    this.x = x;
    this.y = y;
  }

  public override bool Equals(Object other) {
    if (other == null) {
      return false;
    } else if (other.GetType() != typeof(Point)) {
      return false;
    }

    return Equals((Point)other);
  }

  public bool Equals(Point other) {
    return x == other.x && y == other.y;
  }

  public override int GetHashCode() {
    return (x * 0x1f1f1f1f) ^ y;
  }
}