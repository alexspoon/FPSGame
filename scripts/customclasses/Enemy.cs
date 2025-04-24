using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
    public Node ComponentContainer;
    public EnemyHealthComponent HealthComponent;

    public override void _Ready()
    {
        ComponentContainer = GetNode<Node>("Components");
        HealthComponent = ComponentContainer.GetNode<EnemyHealthComponent>("EnemyHealthComponent");
    }
}
