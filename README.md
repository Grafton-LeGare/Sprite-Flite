# Sprite Flite

A simple 2D game in which you fly around in a space ship and avoid deadly obstacles. Built with Unity. Based on Unity's Sprite Flight tutorial

## Play

[Play the game on Unity Play](https://play.unity.com/en/games/17c7bb3d-fb96-4897-86a7-a23760eb5612/sprite-flite)

## Technologies

- Unity 6
- C#
- Unity Input System
- 2D Physics
- UI Toolkit

Web build consideration: Player movement with Rigidbody2D is applied in `FixedUpdate()` rather than `Update()` to maintain consistent physics behavior in the web build
