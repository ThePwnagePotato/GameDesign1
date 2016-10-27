using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tank : Unit
{

	public string _Name;

	public SpriteRenderer _spriteRenderer;

	private Animator _animator;
	public override Animator animator {
		get { return _animator; }
		set { _animator = value; }
	}

	private bool _isAlive;
	public override bool isAlive {
		get { return _isAlive; }
		set { _isAlive = value; }
	}

	public Sprite[] _sprites;

	public override Sprite[] sprites {
		get { return _sprites; }
		set { _sprites = value; }
	}

	public override string getName ()
	{
		return _Name;
	}

	public float _movementSpeed;

	public override float movementSpeed {
		get { return _movementSpeed; }
		set { _movementSpeed = value; }
	}

	public BoardManager _boardManager;

	public override BoardManager boardManager {
		get { return _boardManager; }
		set { _boardManager = value; }
	}

	public GameManager _gameManager;

	public override GameManager gameManager {
		get { return _gameManager; }
		set { _gameManager = value; }
	}

	public bool _isFriendly;

	public override bool isFriendly ()
	{
		return _isFriendly;
	}

	public List<GameObject> _abilities;
	public override List<GameObject> abilities
	{
		get { return _abilities; }
		set { _abilities = value; }
	}

	private bool _canMove;
	public override bool canMove {
		get { return _canMove; }
		set { _canMove = value; }
	}

	private bool _canAttack;
	public override bool canAttack {
		get { return _canAttack; }
		set { _canAttack = value; }
	}

	public int _maxHealth;

	public override int maxHealth {
		get { return _maxHealth; }
		set { _maxHealth = value; }
	}

	private int _currentHealth;

	public override int currentHealth {
		get { return _currentHealth; }
		set { _currentHealth = value; }
	}

	public int _power;

	public override int power {
		get { return _power; }
		set { _power = value; }
	}

	private int _currentPower;

	public override int currentPower {
		get { return _currentPower; }
		set { _currentPower = value; }
	}

	public int _defense;

	public override int defense {
		get { return _defense; }
		set { _defense = value; }
	}

	private int _currentDefense;

	public override int currentDefense {
		get { return _currentDefense; }
		set { _currentDefense = value; }
	}

	public int _totalMoves;

	public override int totalMoves {
		get { return _totalMoves; }
		set { _totalMoves = value; }
	}

	private int _currentMoves;

	public override int currentMoves {
		get { return _currentMoves; }
		set { _currentMoves = value; }
	}

	public int _totalMovesUp;

	public override int totalMovesUp {
		get { return _totalMovesUp; }
		set { _totalMovesUp = value; }
	}

	private int _currentMovesUp;

	public override int currentMovesUp {
		get { return _currentMovesUp; }
		set { _currentMovesUp = value; }
	}

	public int _totalMovesDown;

	public override int totalMovesDown {
		get { return _totalMovesDown; }
		set { _totalMovesDown = value; }
	}

	private int _currentMovesDown;

	public override int currentMovesDown {
		get { return _currentMovesDown; }
		set { _currentMovesDown = value; }
	}

	public int _totalMovesSide;

	public override int totalMovesSide {
		get { return _totalMovesSide; }
		set { _totalMovesSide = value; }
	}

	private int _currentMovesSide;

	public override int currentMovesSide {
		get { return _currentMovesSide; }
		set { _currentMovesSide = value; }
	}


}