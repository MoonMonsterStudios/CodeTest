using UnityEngine;

namespace MoonMonster.Codetest
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private float _dampTime = 0.2f;
        [SerializeField] private float _screenEdgeBuffer = 8;
        [SerializeField] private float _minSize = 13;

        public Transform[] Targets { get; set; }

        private Camera _camera;
        private Vector3 _moveVelocity;
        private Vector3 _desiredPosition;

        private void Awake()
        {
            _camera = GetComponentInChildren<Camera>();
        }

        private void FixedUpdate()
        {
            Move();
            Zoom();
        }

        private void Move()
        {
            FindAveragePosition();

            transform.position = Vector3.SmoothDamp(transform.position, _desiredPosition, ref _moveVelocity, _dampTime);
        }

        private void FindAveragePosition()
        {
            Vector3 averagePos = new Vector3();
            int numTargets = 0;

            for (int i = 0; i < Targets.Length; i++)
            {
                if (!Targets[i].gameObject.activeSelf)
                    continue;

                averagePos += Targets[i].position;
                numTargets++;
            }

            if (numTargets > 0)
                averagePos /= numTargets;

            averagePos.y = transform.position.y;

            _desiredPosition = averagePos;
        }

        private void Zoom()
        {
            float requiredSize = FindRequiredSize();
            float zoomSpeed = 0;
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, requiredSize, ref zoomSpeed, _dampTime);
        }

        private float FindRequiredSize()
        {
            Vector3 desiredLocalPos = transform.InverseTransformPoint(_desiredPosition);
            float size = 0f;

            for (int i = 0; i < Targets.Length; i++)
            {
                if (!Targets[i].gameObject.activeSelf)
                    continue;

                Vector3 targetLocalPos = transform.InverseTransformPoint(Targets[i].position);
                Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / _camera.aspect);
            }

            size += _screenEdgeBuffer;
            size = Mathf.Max(size, _minSize);

            return size;
        }

        public void SetStartPositionAndSize()
        {
            FindAveragePosition();

            transform.position = _desiredPosition;
            _camera.orthographicSize = FindRequiredSize();
        }
    }
}