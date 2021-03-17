using UnityEngine;
using System.Collections;

public static class GameObjectContainers
{
    static Transform _staticBuildings;
    static Transform _staticMisc;

    static Transform _dynamicUnits;
    static Transform _dynamicMisc;
    static Transform _bullets;

    public static Transform StaticBuildings
    {
        get
        {
            if (_staticBuildings != null)
                return _staticBuildings;

            InitStaticObjectsContainer();
            return _staticBuildings;
        }

        private set
        {
            _staticBuildings = value;
        }
    }

    public static Transform StaticMisc
    {
        get
        {
            if (_staticMisc != null)
                return _staticMisc;

            InitStaticObjectsContainer();
            return _staticMisc;
        }

        private set
        {
            _staticMisc = value;
        }
    }

    public static Transform Bullets
    {
        get
        {
            if (_bullets != null)
                return _bullets;

           InitDynamicObjectsContainer();
           return _bullets;
        }

        private set
        {
            _bullets = value;
        }
    }

    public static Transform DynamicUnits
    {
        get
        {
            if (_dynamicUnits != null)
                return _dynamicUnits;

            InitDynamicObjectsContainer();
            return _dynamicUnits;
        }

        private set
        {
            _dynamicUnits = value;
        }
    }

    public static Transform DynamicMisc
    {
        get
        {
            if (_dynamicMisc != null)
                return _dynamicMisc;

            InitDynamicObjectsContainer();
            return _dynamicMisc;
        }

        private set
        {
            _dynamicMisc = value;
        }
    }



    public static void InitAllContainers()
    {
        InitStaticObjectsContainer();
        InitDynamicObjectsContainer();
    }

    static void InitStaticObjectsContainer()
    {
        var go = GameObject.Find("StaticObjects");
        Transform staticObjects;
        if (go == null)
            staticObjects = new GameObject("StaticObjects").transform;
        else
            staticObjects = go.transform;


        if (_staticBuildings == null)
        {
            var buildingsGO = staticObjects.FindChild("Buildings");
            if (buildingsGO == null)
                buildingsGO = new GameObject("Buildings").transform;
            _staticBuildings = buildingsGO;
            _staticBuildings.parent = staticObjects;
        }

        if (_staticMisc == null)
        {
            var miscGO = staticObjects.FindChild("Misc");
            if (miscGO == null)
                miscGO = new GameObject("Misc").transform;
            _staticMisc = miscGO;
            _staticMisc.parent = staticObjects;
        }
    }

    static void InitDynamicObjectsContainer()
    {
        var go = GameObject.Find("DynamicObjects");
        Transform dynamicObjects;
        if (go == null)
            dynamicObjects = new GameObject("DynamicObjects").transform;
        else
            dynamicObjects = go.transform;

        if (_dynamicUnits == null)
        {
            var unitsGO = dynamicObjects.FindChild("Units");
            if (unitsGO == null)
                unitsGO = new GameObject("Units").transform;
            _dynamicUnits = unitsGO;
            _dynamicUnits.parent = dynamicObjects;
        }

        if (_dynamicMisc == null)
        {
            var miscGO = dynamicObjects.FindChild("Misc");
            if (miscGO == null)
                miscGO = new GameObject("Misc").transform;
            _dynamicMisc = miscGO;
            _dynamicMisc.parent = dynamicObjects;
        }

        if (_bullets == null)
        {
            var bulletsGO = dynamicObjects.FindChild("Bullets");
            if (bulletsGO == null)
                bulletsGO = new GameObject("Bullets").transform;
            _bullets = bulletsGO;
            _bullets.parent = dynamicObjects;
        }
    }


}
