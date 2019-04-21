using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviours : MonoBehaviour {

    public static Behaviours instance;

    // the following variables are references to possible targets
    public GameObject banana;
    public GameObject cookie;
    public GameObject apple;
    internal GameObject gazedTarget;

    void Awake()
    {
        // allows this class instance to behave like a singleton
        instance = this;
    }


    /// <summary>
    /// Show more target GameObjects by providing the name of the object
    /// </summary>
    public void Show(string targetName)
    {
        GameObject foundTarget = FindTarget(targetName);
        if (foundTarget != null)
        {
            StartCoroutine(BulkCreation(foundTarget));
        }
    }

    /// <summary>
    /// Hide the target GameObject by providing the name of the object
    /// </summary>
    public void Hide(string targetName)
    {
        GameObject foundTarget = FindTarget(targetName);
        if (foundTarget != null)
        {
            StartCoroutine(CreateWall(foundTarget));
        }
    }

    /// <summary>
    /// Reduces the size of the target GameObject by providing its name
    /// </summary>
    public void DownSizeTarget(string targetName)
    {
        GameObject foundTarget = FindTarget(targetName);
        foundTarget.transform.localScale -= new Vector3(1F, 1F, 1F);
    }

    /// <summary>
    /// Increases the size of the target GameObject by providing its name
    /// </summary>
    public void UpSizeTarget(string targetName)
    {
        GameObject foundTarget = FindTarget(targetName);
        foundTarget.transform.localScale += new Vector3(1F, 1F, 1F);
    }



    /// <summary>
    /// Determines which obejct reference is the target GameObject by providing its name
    /// </summary>
    private GameObject FindTarget(string name)
    {
        GameObject targetAsGO = null;

        switch (name)
        {
            case "banana":
                targetAsGO = banana;
                break;

            case "cookie":
                targetAsGO = cookie;
                break;

            case "apple":
                targetAsGO = apple;
                break;

            case "this": // as an example of target words that the user may use when looking at an object
            case "it":  // as this is the default, these are not actually needed in this example
            case "that":
            default: // if the target name is none of those above, check if the user is looking at something
                if (gazedTarget != null)
                {
                    targetAsGO = gazedTarget;
                }
                break;
        }
        return targetAsGO;
    }





    private IEnumerator BulkCreation(GameObject prefab)
    {
        for (int i = 0; i < 80; i++)
        {
            StartCoroutine(CreateObject(prefab));
            yield return new WaitForSeconds(0.05f);
        }
        yield return 1;
    }

    private IEnumerator CreateObject(GameObject prefab)
    {
        var o = Instantiate(prefab, new Vector3(0f, 10f, 8f), UnityEngine.Random.rotation);
        yield return 1;
    }


    private IEnumerator CreateWall(GameObject target)
    {
        var o = GameObject.CreatePrimitive(PrimitiveType.Cube);
        o.name = System.Guid.NewGuid().ToString();
        o.transform.localScale = new Vector3(2f, 2f, 0.5f);
        //o.transform.position = new Vector3(-0.13f, 3f, 7.2f);
        o.transform.position = target.transform.localPosition - Vector3.back * 0.1f;
        o.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        o.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, 0.6f, 1f, 1f, 1f, 0f, 0.2f);
        o.AddComponent<Rigidbody>();
        yield return 1;
    }
}
