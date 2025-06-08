using Gley.TrafficSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gley.UrbanSystem.Internal
{
    public class UrbanExample : MonoBehaviour
    {
        [SerializeField] private Transform _busStops;
        private bool _pathSet;
        private int _stopNumber;
        private bool _followVehicle;
        private Transform _player;

        private const int _vehicleToFollow = 23;

        private void Start()
        {
            _player = GameObject.Find("Player").transform;
        }

        //every time a destination is reached, a new one is selected
        private void BusStationReached(int vehicleIndex)
        {
            //remove listener otherwise this method will be called on each frame
            TrafficSystem.Events.OnDestinationReached -= BusStationReached;
            if (vehicleIndex == 0)
            {
                _stopNumber++;
                if (_stopNumber == _busStops.childCount)
                {
                    _stopNumber = 0;
                }
                //stop and wait for 5 seconds, then move to the next destination
                Invoke("ContinueDriving", 5);
            }
        }

        /// <summary>
        /// Continue on path
        /// </summary>
        private void ContinueDriving()
        {
            TrafficSystem.Events.OnDestinationReached += BusStationReached;
            TrafficSystem.API.SetDestination(0, _busStops.GetChild(_stopNumber).transform.position);
        }

        private void Update()
        {
            if (!_pathSet)
            {
                if (TrafficSystem.API.IsInitialized())
                {
                    _pathSet = true;
                    SetPath();
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                _followVehicle = !_followVehicle;
                if (_followVehicle)
                {
                    GameObject.Find("Main Camera").GetComponent<CameraFollow>().target = TrafficSystem.API.GetVehicleComponent(_vehicleToFollow).transform;
                    TrafficSystem.API.SetCamera(TrafficSystem.API.GetVehicleComponent(_vehicleToFollow).transform);
                    PedestrianSystem.API.SetCamera(TrafficSystem.API.GetVehicleComponent(_vehicleToFollow).transform);
                }
                else
                {
                    GameObject.Find("Main Camera").GetComponent<CameraFollow>().target = _player;
                    TrafficSystem.API.SetCamera(_player);
                    PedestrianSystem.API.SetCamera(_player);
                }
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                API.StartVehicleBehaviour<IgnoreTrafficRules>(_vehicleToFollow);
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                API.StopVehicleBehaviour<IgnoreTrafficRules>(_vehicleToFollow);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(0);
            }
        }

        /// <summary>
        /// set a path towards destination
        /// </summary>
        private void SetPath()
        {
            var vehicleComponent = TrafficSystem.API.GetVehicleComponent(0);
            if (vehicleComponent.gameObject.activeSelf)
            {
                TrafficSystem.Events.OnDestinationReached += BusStationReached;
                TrafficSystem.API.SetDestination(0, _busStops.GetChild(_stopNumber).transform.position);
            }
            else
            {
                Invoke("SetPath", 1);
            }
        }

        //remove listeners
        private void OnDestroy()
        {
            TrafficSystem.Events.OnDestinationReached -= BusStationReached;
        }
    }
}
