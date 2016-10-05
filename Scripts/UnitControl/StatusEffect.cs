using UnityEngine;
using System.Collections;

public abstract class StatusEffect : MonoBehaviour {

	public abstract string GetName ();
	public abstract bool IsPositive ();
	public abstract EffectType GetEffectType ();

	public abstract int power { get; set; }
	public abstract int duration { get; set; }

	public StatusEffect (int power, int duration) {
		this.power = power;
		this.duration = duration;
	}

	// Lowers duration by one. If 0 or less afterwards, return true
	public bool UpdateDuration () {
		return --duration <= 0;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public enum EffectType {
	DAMAGING,
	HEALING,

	POWDEBUFF,
	DEFDEBUFF,
	MOVDEBUFF,

	POWBUFF,
	DEFBUFF,
	MOVBUFF,
}