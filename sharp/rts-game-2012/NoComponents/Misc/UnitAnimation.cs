using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// класс для  оптимизации работы с анимацией юнитов. 
/// </summary>
public class UnitAnimation{

    public sealed class States
    {
        private readonly string name;

        public static readonly States None = new States("None");
        public static readonly States Idle = new States("idle");
        public static readonly States Run = new States("run");
        public static readonly States IdleBattle = new States("idlebattle");
        public static readonly States Attack = new States("attack");
        public static readonly States Die = new States("die");

        private States(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }

    public Animation animation;

    States _animationState;

    public States State
    {
        get
        {
            return _animationState;
        }

        set
        {
            //т.к. анимация атаки не циклическая, то разрешен повторный вызов Play
            if (value == States.Attack)
            {
                _animationState = value;
                animation.CrossFade(value.ToString());
                return;
            }

            //в остальных случаях запрещаем повторный вызов Play 
            if (_animationState != value && value != States.None)
            {
                _animationState = value;
                animation.CrossFade(value.ToString());
            }
        
        }
    }

    public UnitAnimation(Animation original)
    {
        animation = original;
    }

    public float GetCurrentClipLength()
    {
        return this.animation[this.State.ToString()].clip.length;
    }
}
