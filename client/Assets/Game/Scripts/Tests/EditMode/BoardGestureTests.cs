using DuckDoku.Presentation;
using NUnit.Framework;

namespace DuckDoku.Tests
{
    [TestFixture]
    public class BoardGestureTests
    {
        private const int Pointer = 0;
        private const int OtherPointer = 1;

        private const float DoubleTapSeconds = 0.22f;
        private const float StaleTimeoutSeconds = 999f;

        private FakeGestureBoard _board;
        private BoardGesture _gesture;

        [SetUp]
        public void SetUp()
        {
            _board = new FakeGestureBoard();
            _gesture = new BoardGesture(_board, DoubleTapSeconds, StaleTimeoutSeconds);
        }

        [Test]
        public void Tap_OnEmptyCell_PutsMark()
        {
            Tap(2, 2, 1f);

            Assert.That(_board.At(2, 2), Is.EqualTo(FakeCell.Cross));
        }

        [Test]
        public void Tap_OnMarkedCell_ErasesMark()
        {
            _board.Set(2, 2, FakeCell.Cross);

            Tap(2, 2, 1f);

            Assert.That(_board.At(2, 2), Is.EqualTo(FakeCell.Empty));
        }

        [Test]
        public void DoubleTap_PlacesDuck()
        {
            Tap(3, 3, 1f);
            Tap(3, 3, 1.1f);

            Assert.That(_board.Log, Does.Contain("duck 3:3"));
        }

        [Test]
        public void DoubleTap_DoesNotEraseMarkBetweenTaps()
        {
            Tap(3, 3, 1f);

            _gesture.Press(Pointer, 3, 3, 1.1f);

            Assert.That(_board.At(3, 3), Is.EqualTo(FakeCell.Cross));

            _gesture.Release(Pointer, 1.15f);

            Assert.That(_board.Log, Is.EqualTo(new[] { "mark 3:3=x", "duck 3:3" }));
        }

        [Test]
        public void SecondTap_AfterWindowExpired_IsPlainTap()
        {
            Tap(3, 3, 1f);
            Tap(3, 3, 5f);

            Assert.That(_board.At(3, 3), Is.EqualTo(FakeCell.Empty));
            Assert.That(_board.Log, Does.Not.Contain("duck 3:3"));
        }

        [Test]
        public void TwoTaps_OnDifferentCells_DoNotPlaceDuck()
        {
            Tap(3, 3, 1f);
            Tap(3, 4, 1.05f);

            Assert.That(_board.Log, Does.Not.Contain("duck 3:4"));
        }

        [Test]
        public void Swipe_MarksWholePath()
        {
            _gesture.Press(Pointer, 0, 0, 1f);
            _gesture.Enter(Pointer, 0, 1);
            _gesture.Enter(Pointer, 0, 2);
            _gesture.Release(Pointer, 1.2f);

            Assert.That(_board.At(0, 0), Is.EqualTo(FakeCell.Cross));
            Assert.That(_board.At(0, 1), Is.EqualTo(FakeCell.Cross));
            Assert.That(_board.At(0, 2), Is.EqualTo(FakeCell.Cross));
        }

        [Test]
        public void Swipe_StartedOnMark_ErasesPath()
        {
            _board.Set(0, 0, FakeCell.Cross);
            _board.Set(0, 1, FakeCell.Cross);

            _gesture.Press(Pointer, 0, 0, 1f);
            _gesture.Enter(Pointer, 0, 1);
            _gesture.Release(Pointer, 1.2f);

            Assert.That(_board.At(0, 0), Is.EqualTo(FakeCell.Empty));
            Assert.That(_board.At(0, 1), Is.EqualTo(FakeCell.Empty));
        }

        [Test]
        public void Swipe_FillsCellsSkippedBetweenFrames()
        {
            _gesture.Press(Pointer, 0, 0, 1f);
            _gesture.Enter(Pointer, 0, 4);
            _gesture.Release(Pointer, 1.2f);

            for (int column = 0; column <= 4; column++)
            {
                Assert.That(_board.At(0, column), Is.EqualTo(FakeCell.Cross), $"клетка 0:{column}");
            }
        }

        [Test]
        public void Swipe_SkipsLockedCellAndKeepsGoing()
        {
            _board.Set(0, 2, FakeCell.Locked);

            _gesture.Press(Pointer, 0, 0, 1f);
            _gesture.Enter(Pointer, 0, 4);
            _gesture.Release(Pointer, 1.2f);

            Assert.That(_board.At(0, 2), Is.EqualTo(FakeCell.Locked));
            Assert.That(_board.At(0, 3), Is.EqualTo(FakeCell.Cross));
            Assert.That(_board.At(0, 4), Is.EqualTo(FakeCell.Cross));
        }

        [Test]
        public void Swipe_TakesModeFromFirstEditableCell()
        {
            _board.Set(0, 0, FakeCell.Locked);
            _board.Set(0, 1, FakeCell.Cross);

            _gesture.Press(Pointer, 0, 0, 1f);
            _gesture.Enter(Pointer, 0, 1);
            _gesture.Enter(Pointer, 0, 2);
            _gesture.Release(Pointer, 1.2f);

            Assert.That(_board.At(0, 1), Is.EqualTo(FakeCell.Empty));
            Assert.That(_board.At(0, 2), Is.EqualTo(FakeCell.Empty));
        }

        [Test]
        public void Tap_AfterSwipe_IsNotCountedAsSecondTap()
        {
            _gesture.Press(Pointer, 1, 1, 1f);
            _gesture.Enter(Pointer, 1, 2);
            _gesture.Release(Pointer, 1.05f);

            Tap(1, 1, 1.1f);

            Assert.That(_board.Log, Does.Not.Contain("duck 1:1"));
        }

        [Test]
        public void Swipe_StartedWhileWaitingForSecondTap_MarksStartCell()
        {
            Tap(5, 5, 1f);
            _board.Set(5, 5, FakeCell.Empty);

            _gesture.Press(Pointer, 5, 5, 1.05f);
            _gesture.Enter(Pointer, 5, 6);
            _gesture.Release(Pointer, 1.2f);

            Assert.That(_board.At(5, 5), Is.EqualTo(FakeCell.Cross));
            Assert.That(_board.At(5, 6), Is.EqualTo(FakeCell.Cross));
            Assert.That(_board.Log, Does.Not.Contain("duck 5:5"));
        }

        [Test]
        public void SecondPointer_IsIgnoredWhileFirstIsDrawing()
        {
            _gesture.Press(Pointer, 0, 0, 1f);
            _gesture.Press(OtherPointer, 7, 7, 1.01f);
            _gesture.Enter(OtherPointer, 7, 6);
            _gesture.Enter(Pointer, 0, 1);
            _gesture.Release(Pointer, 1.2f);

            Assert.That(_board.At(7, 7), Is.EqualTo(FakeCell.Empty));
            Assert.That(_board.At(7, 6), Is.EqualTo(FakeCell.Empty));
            Assert.That(_board.At(0, 0), Is.EqualTo(FakeCell.Cross));
            Assert.That(_board.At(0, 1), Is.EqualTo(FakeCell.Cross));
            Assert.That(_board.At(4, 4), Is.EqualTo(FakeCell.Empty), "линия между пальцами");
        }

        [Test]
        public void Release_FromForeignPointer_DoesNotEndGesture()
        {
            _gesture.Press(Pointer, 0, 0, 1f);
            _gesture.Release(OtherPointer, 1.05f);
            _gesture.Enter(Pointer, 0, 1);
            _gesture.Release(Pointer, 1.2f);

            Assert.That(_board.At(0, 1), Is.EqualTo(FakeCell.Cross));
        }

        [Test]
        public void Reset_ForgetsPendingTap()
        {
            Tap(3, 3, 1f);

            _gesture.Reset();

            Tap(3, 3, 1.05f);

            Assert.That(_board.Log, Does.Not.Contain("duck 3:3"));
        }

        [Test]
        public void Tick_BeforeStaleTimeout_KeepsGestureActive()
        {
            _gesture.Press(Pointer, 0, 0, 1f);

            _gesture.Tick(1f + StaleTimeoutSeconds - 0.01f);

            _gesture.Press(Pointer, 5, 5, 1f + StaleTimeoutSeconds);

            Assert.That(_board.At(5, 5), Is.EqualTo(FakeCell.Empty));
        }

        [Test]
        public void Tick_AfterStaleTimeout_ReleasesStuckGesture()
        {
            _gesture.Press(Pointer, 0, 0, 1f);

            _gesture.Tick(1f + StaleTimeoutSeconds + 0.01f);

            _gesture.Press(Pointer, 5, 5, 1f + StaleTimeoutSeconds + 0.02f);

            Assert.That(_board.At(5, 5), Is.EqualTo(FakeCell.Cross));
        }

        private void Tap(int row, int column, float time)
        {
            _gesture.Press(Pointer, row, column, time);
            _gesture.Release(Pointer, time + 0.02f);
        }
    }
}
