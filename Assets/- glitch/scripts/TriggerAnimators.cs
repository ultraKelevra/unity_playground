using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAnimators : MonoBehaviour
{
	private Animator[] anims;

	public string TriggerName = "Glitch";
	
	// Use this for initialization
	void Start ()
	{
		anims = GetComponents<Animator>();
	}

	public void Trigger()
	{
		foreach (var item in anims)
		{
			item.SetTrigger(TriggerName);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
