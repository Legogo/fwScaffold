using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class AnimatorParameter
{
    protected bool _active = false;
    protected string paramUID;
    protected Animator _animator;

    public AnimatorParameter(Animator tarAnimator, string paramUID)
    {
        _animator = tarAnimator;
        Debug.Assert(_animator, "need animator");

        this.paramUID = paramUID;

        for (int i = 0; i < _animator.parameterCount; i++)
        {
            var param = _animator.GetParameter(i);
            if (param.name == paramUID) _active = true;
        }

        if (!_active) Debug.LogWarning(_animator + " doesnt have param : " + paramUID, _animator);
    }

    /// <summary>
    /// this animator HAS this parameter ?
    /// </summary>
    public bool isActive() => _active;

    public string stringify()
    {
        string output = GetType().ToString() + "&" + paramUID;
        output += "&active?" + _active;
        if (_active) output += "&" + stringifyValue();
        return output;
    }

    abstract public string stringifyValue();
}

class AnimatorParameterBool : AnimatorParameter
{
    public AnimatorParameterBool(Animator tarAnimator, string paramUID) : base(tarAnimator, paramUID)
    {
    }

    public void injectState(bool newState)
    {
        if (!_active) return;

        bool local = _animator.GetBool(paramUID);
        if (local != newState)
        {
            _animator.SetBool(paramUID, newState);
            //Debug.Log($"AnimatorParameterBool {paramUID} => {newState}");
        }
    }

    public override string stringifyValue()
    {
        if (!_active) return string.Empty;
        return _animator.GetBool(paramUID).ToString();
    }
}

class AnimatorParameterInt : AnimatorParameter
{
    public AnimatorParameterInt(Animator tarAnimator, string paramUID) : base(tarAnimator, paramUID)
    {
    }

    public void update(int newState)
    {
        if (!_active) return;

        int local = _animator.GetInteger(paramUID);
        if (local != newState)
        {
            _animator.SetInteger(paramUID, newState);
            //Debug.Log(paramUID + " => " + newState);
        }
    }

    public override string stringifyValue()
    {
        if (!_active) return string.Empty;
        return _animator.GetInteger(paramUID).ToString();
    }
}

class AnimatorParameterFloat : AnimatorParameter
{
    public AnimatorParameterFloat(Animator tarAnimator, string paramUID) : base(tarAnimator, paramUID)
    {
    }

    public void update(float newState)
    {
        if (!_active) return;

        float local = _animator.GetFloat(paramUID);
        if (local != newState)
        {
            _animator.SetFloat(paramUID, newState);
            //Debug.Log(paramUID + " => " + newState);
        }
    }

    public override string stringifyValue()
    {
        if (!_active) return string.Empty;
        return _animator.GetFloat(paramUID).ToString();
    }
}

class AnimatorParameterTrigger : AnimatorParameter
{
    public AnimatorParameterTrigger(Animator tarAnimator, string paramUID) : base(tarAnimator, paramUID)
    {
    }

    public void trigger()
    {
        if (!_active) return;

        _animator.SetTrigger(paramUID);
    }

    public override string stringifyValue() => string.Empty;
}
