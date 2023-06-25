using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// blueprint for animation capacity integration
/// 
/// Animator + ANimatorController
/// </summary>

namespace brainer
{
  abstract public class CapacityAnimation : BrainerLogicCapacity
  {
    protected Animator _anim;
    protected bool _animCaptured = false;

    protected string overrideNameAnimation = "";

    public override void setupCapacity()
    {
      base.setupCapacity();

      _anim = gameObject.GetComponentsInChildren<Animator>().FirstOrDefault();
    }

    public override void updateCapacity()
    {
      updateAnimation();
    }

    abstract protected void updateAnimation();

    public void pause()
    {
    }
    public void resume()
    { }

    public void PlayAnimOfName(string animName)
    {
      //Debug.Log(name+" playing " + animName);

      if (_anim == null) return;

      _anim.Play(animName);
    }

    public bool isPlaying(string animName)
    {
      if (_anim == null) return false;
      AnimatorStateInfo state = _anim.GetCurrentAnimatorStateInfo(0);
      if (state.IsName(animName)) return true;
      return false;
    }

    /* anim won't play something else */
    public void lockAnimation()
    {
      _animCaptured = true;
    }

    public void releaseAnimation()
    {
      _animCaptured = false;
    }


    /* anim won't play something else */
    public void captureAnim(string newAnimationName)
    {
      overrideNameAnimation = newAnimationName;
      //_anim.Play(overrideNameAnimation);
    }

    public void releaseAnim()
    {
      overrideNameAnimation = "";
    }

  }
}