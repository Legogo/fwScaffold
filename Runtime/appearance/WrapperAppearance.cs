using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.Rendering;

namespace fwp.scaffold
{
	public class WrapperAppearance
	{
		// toki J'ai remplac� "tinies-selected" par "dp-default"
		// pour pas que les tinies passent devant un tas de trucs,
		// je sais pas si on s'�tait mal compris mais ils doivent toujours rester sur ce layer
		//private const string SELECTED_LAYER_NAME = "gp-default";

		// deprecated stuff ?
		private int selectedLayerId = SortingLayer.NameToID("gp-default");
		private int layerGameplayUi = SortingLayer.NameToID("ui");

		private enum AnimatorType { Bool, Int, Float, Trigger }

		protected Animator wrapperAnimator;
		private Dictionary<string, AnimatorParameter> parameters;

		// this pivot is extracted when solving for animator
		protected MonoBehaviour owner;

		public Transform pivotSymbol;
		protected SortingGroup sortingGroup;

		struct AppearanceSortingGroup
		{
			public int layer;
			public int order;

			public void apply(SortingGroup group)
			{
				group.sortingLayerID = layer;
				group.sortingOrder = order;
			}
		}

		AppearanceSortingGroup aSortingGroup = new AppearanceSortingGroup();

		public WrapperAppearance(MonoBehaviour owner)
		{
			this.owner = owner;

			fetch();
		}

		public void fetch(bool force = false)
		{
			//Debug.Log(owner.name + " APPEARANCE:fetch(" + force + ")", owner);

			if (pivotSymbol == null || force)
			{
				pivotSymbol = getPivot(); // "Visual"
			}

			Debug.Assert(pivotSymbol, "no pivot ?");

			if (parameters == null)
				parameters = new Dictionary<string, AnimatorParameter>();

			if (wrapperAnimator == null || force)
			{
				wrapperAnimator = ExtractAnimator();

				if (wrapperAnimator != null)
					pivotSymbol = wrapperAnimator.transform;

			}

			if (wrapperAnimator == null)
			{
				Debug.LogWarning("no animator on " + owner, owner);
				//Debug.Assert(animator != null, " need animator on " + owner, owner);
			}

			sortingGroup = pivotSymbol.GetComponentInChildren<SortingGroup>();
			if (sortingGroup != null)
			{
				// backup default
				aSortingGroup.layer = sortingGroup.sortingLayerID;
				aSortingGroup.order = sortingGroup.sortingOrder;
			}
			//else Debug.LogWarning(owner.name + " NO sorting group ?", owner);
		}

		/// <summary>
		/// itself
		/// or "Visual" in other subclass
		/// </summary>
		virtual protected Transform getPivot()
		{
			return owner.transform;
		}

		/// <summary>
		/// was init properly
		/// </summary>
		public bool IsValid() => wrapperAnimator != null;

		public Animator getAnimator()
		{
			return wrapperAnimator;
		}

		/// <summary>
		/// meant to EXTRACT
		/// not return after already solved
		/// </summary>
		private Animator ExtractAnimator()
		{
			Animator output = pivotSymbol.GetComponent<Animator>(); ;

			if (output == null)
				output = pivotSymbol.GetComponentInChildren<Animator>();

			return output;
		}

		public void swapRig(int index)
		{
			for (int i = 0; i < wrapperAnimator.transform.childCount; i++)
			{
				wrapperAnimator.transform.GetChild(i).gameObject.SetActive(i == 0);
			}
		}

		public void SetSpeed(float value)
		{
			wrapperAnimator.speed = value;
		}

		public void SetBool(string id, bool value)
		{
			var param = (AnimatorParameterBool)GetParam(id, AnimatorType.Bool);
			param.injectState(value);
		}

		public void SetInt(string id, int value)
		{
			var param = (AnimatorParameterInt)GetParam(id, AnimatorType.Int);
			param.update(value);
		}

		public void SetFloat(string id, float value)
		{
			var param = (AnimatorParameterFloat)GetParam(id, AnimatorType.Float);
			param.update(value);
		}

		public bool SetTrigger(string id)
		{
			var param = (AnimatorParameterTrigger)GetParam(id, AnimatorType.Trigger);
			if (param == null)
			{
				Debug.LogWarning("[inte] no trigger " + id);
				return false;
			}
			param.trigger();
			return true;
		}

		private AnimatorParameter GetParam(string id, AnimatorType type)
		{
			Debug.Assert(wrapperAnimator != null, "no animator when getting param " + id, owner);

			if (parameters.ContainsKey(id))
				return parameters[id];

			switch (type)
			{
				case AnimatorType.Bool:
					parameters.Add(id, new AnimatorParameterBool(wrapperAnimator, id));
					break;

				case AnimatorType.Int:
					parameters.Add(id, new AnimatorParameterInt(wrapperAnimator, id));
					break;

				case AnimatorType.Float:
					parameters.Add(id, new AnimatorParameterFloat(wrapperAnimator, id));
					break;

				default:
					parameters.Add(id, new AnimatorParameterTrigger(wrapperAnimator, id));
					break;
			}

			return parameters[id];
		}

		public bool GetBool(string id)
		{
			return wrapperAnimator.GetBool(id);
		}

		public void SetStateProgress(string layerName, float progress)
		{
			int layerIndex = wrapperAnimator.GetLayerIndex(layerName);
			var stateInfo = wrapperAnimator.GetCurrentAnimatorStateInfo(layerIndex);
			wrapperAnimator.Play(stateInfo.fullPathHash, layerIndex, progress);
		}

		public AnimatorStateInfo GetCurrentAnimatorStateInfo(string layerName)
		{
			int layerIndex = wrapperAnimator.GetLayerIndex(layerName);
			return wrapperAnimator.GetCurrentAnimatorStateInfo(layerIndex);
		}

		public void FlipVisual(bool value)
		{
			Vector3 scaleToApply = pivotSymbol.localScale;
			scaleToApply.x = value ? -1f : 1f;
			pivotSymbol.localScale = scaleToApply;
		}

		public void SetSortingLayer(int layer)
		{
			sortingGroup.sortingLayerID = layer;
		}

		public virtual void SetSortingOrder(int value)
		{
			sortingGroup.sortingOrder = value;
		}

		public void SetSortingLayerToGameplay(int? order = null)
		{
			if (sortingGroup.sortingLayerID != selectedLayerId)
				sortingGroup.sortingLayerID = selectedLayerId;

			if (order != null) sortingGroup.sortingOrder = order.Value;
		}

		public void SetSortingLayerToUi(int? order = null)
		{
			if (sortingGroup == null)
			{
				Debug.LogWarning(owner + " :: trying to assign sorting layer to null sorting group ?", owner);
				Debug.Log(owner, owner);
				Debug.Log(wrapperAnimator, wrapperAnimator);
				Debug.Log(sortingGroup, sortingGroup);
				return;
			}

			if (sortingGroup.sortingLayerID != layerGameplayUi)
				sortingGroup.sortingLayerID = layerGameplayUi;

			if (order != null) sortingGroup.sortingOrder = order.Value;
		}

		/// <summary>
		/// re-apply default values of sorting group
		/// </summary>
		public void ResetSortingGroup()
		{
			aSortingGroup.apply(sortingGroup);
		}

		public int GetSortingOrger()
		{
			return sortingGroup.sortingOrder;
		}

		/// <summary>
		/// DEPRECATED
		/// this can be used by any IStateowner
		/// </summary>
		protected Transform applyChanglingState(string stateName)
		{
			//Debug.Log(this.name+" toggle changling : " + stateName, this);
			Transform curPivot = null;

			//BrainBase.toggleChangeling(this, stateName);

			wrapperAnimator = null;
			if (curPivot != null)
				wrapperAnimator = curPivot.GetComponent<Animator>();

			return curPivot;
		}

		/// <summary>
		/// launch an anim by triggering the animator
		/// and wait for a specific state before bubbling
		/// </summary>
		public void playTriggerAnimation(string triggerId, string endState, Action onCompletion = null)
		{
			var param = GetParam(triggerId, AnimatorType.Trigger);

			// filter if trigger doesn't exists
			if (!param.isActive())
			{
				Debug.LogWarning("triggering " + triggerId + " but no param for it");
				onCompletion?.Invoke();
				return;
			}

			(param as AnimatorParameterTrigger).trigger();

			// watch for end
			owner.StartCoroutine(playingAnimation(endState, onCompletion));
		}

		/// <summary>
		/// watcher
		/// </summary>
		IEnumerator playingAnimation(string endState, Action atState)
		{
			Debug.Log("trigger :: now watching " + endState);

			AnimatorStateInfo state = wrapperAnimator.GetCurrentAnimatorStateInfo(0);

			// wait for state
			while (!state.IsName(endState))
			{
				state = wrapperAnimator.GetCurrentAnimatorStateInfo(0);
				yield return null;
			}

			atState.Invoke();

			/*
			// wait for ending of state
			while (state.IsName(endState))
			{
				state = animator.GetCurrentAnimatorStateInfo(0);
				yield return null;
			}

			onCompletion?.Invoke();
			*/
		}

		virtual public string stringify()
		{
			return owner.name + " / " + getPivot();
		}
	}

}
