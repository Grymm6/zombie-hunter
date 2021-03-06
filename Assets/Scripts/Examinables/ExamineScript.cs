using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExamineScript : MonoBehaviour
{

	public GameObject prefabParent;
	public GameObject prefabPopUp;
	public float WaitTime = 0.2f;
	public MyCharacterController characterController;
	public float raycastDistance = 0.16f;
	private GameObject instantiatedPrefab = null;
	private float startTime;
	private float objectSizeX = 0.08f;
	private float objectSizeY = 0.16f;

	void Start ()
	{
		startTime = Time.time;
		if (characterController == null) {
			characterController = GetComponent<MyCharacterController> ();
			if (characterController == null) {
				throw new MissingComponentException ("MyCharacterController missing");
			}
		}
		BoxCollider2D boxCollider = GetComponent<BoxCollider2D> ();
		if (boxCollider != null) {
			objectSizeX = boxCollider.size.x;
			objectSizeY = boxCollider.size.y;
		}
	}

	// Update is called once per frame
	void Update ()
	{
//		Debug.DrawRay (new Vector2 (transform.position.x + characterController.direction.x * objectSizeX, transform.position.y + characterController.direction.y * objectSizeY), new Vector2 (characterController.direction.x, characterController.direction.y) * raycastDistance, Color.green);
		if (Input.GetButton ("Fire2")) {
			if (Time.time - startTime > WaitTime) {
				RaycastHit2D hit = Physics2D.Raycast (new Vector2 (transform.position.x + characterController.direction.x * objectSizeX, transform.position.y + characterController.direction.y * objectSizeY), new Vector2 (characterController.direction.x, characterController.direction.y) * raycastDistance, raycastDistance);

				if (instantiatedPrefab == null) {
					if (hit.collider != null) {
						Examinable examinableObject = hit.collider.gameObject.GetComponent<Examinable> ();
						if (examinableObject != null) {
							startTime = Time.time;
							instantiatedPrefab = (GameObject)Instantiate (prefabPopUp, new Vector3 (0, 0, 0), Quaternion.identity);

							instantiatedPrefab.transform.SetParent (prefabParent.transform);
							instantiatedPrefab.transform.localScale = new Vector3 (1, 1, 1);
							instantiatedPrefab.transform.localPosition = new Vector3 (0, 0, 0);

							GameObject TitleObject = GameObject.Find ("TitleText");

							if (TitleObject != null) {
								Text tile = TitleObject.GetComponent<Text> ();
								if (tile != null) {
									tile.text = examinableObject.Title;
								}
							}

							GameObject TextObject = GameObject.Find ("Text");
							if (TextObject != null) {
								Text text = TextObject.GetComponent<Text> ();
								if (text != null) {
									text.text = examinableObject.Description;
								}
							}

						}
					}
				} else {
					startTime = Time.time;
					DestroyObject (instantiatedPrefab);
				}
			}
		}	
	}
}
