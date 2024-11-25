using System;
using System.Reflection;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Den.Tools;
using MapMagic.Products;
using MapMagic.Expose;


namespace MapMagic.Nodes.Biomes
{
	public abstract class BaseFunctionGenerator : Generator
	/// Separate class mainly for gui purpose, to not distinguish between fn, loop and cluster
	{
		public IFnInlet<object>[] inlets = new IFnInlet<object>[0];
		public IEnumerable<IInlet<object>> Inlets() => inlets;

		public IFnOutlet<object>[] outlets = new IFnOutlet<object>[0];
		public IEnumerable<IOutlet<object>> Outlets() => outlets;

		public Graph subGraph;
		public Graph SubGraph => subGraph;

		public Override ovd = new Override();
		public Override Override { get => ovd; set => ovd=value; }

		[NonSerialized] public Graph guiPrevGraph = null;
		[NonSerialized] public ulong guiPrevGraphVersion = 0;
		[NonSerialized] public string guiSameInletsName = null;
		[NonSerialized] public string guiSameOutletsName = null;
	}


	[Serializable]
	[GeneratorMenu (menu="Functions", name ="Function", iconName="GeneratorIcons/Function", priority = 1, colorType = typeof(IBiome))]
	public class Function210 : BaseFunctionGenerator, IMultiInlet, IMultiOutlet, IPrepare, IBiome, ICustomComplexity, ICustomClear, IRelevant
	//relevant since it could be used as biome
	{
		public float Complexity => subGraph!=null ? subGraph.GetGenerateComplexity() : 0;
		public float Progress (TileData data)
		{
			if (subGraph == null) return 0;

			TileData subData = data.GetSubData(id);
			if (subData == null) return 0;

			return subGraph.GetGenerateProgress(subData);
		}


		public void Prepare (TileData data, Terrain terrain)
		{
			if (subGraph == null) return;
			
			TileData subData = data.CreateLoadSubData(id);

			subGraph.Prepare(subData, terrain);
		}


		public override void Generate (TileData data, StopToken stop)
		{
			if (stop!=null && stop.stop) return;
			if (subGraph == null) return;

			TileData subData = data.CreateLoadSubData(id);

			//sending inlet products to sub-graph enters
			if (stop!=null && stop.stop) return;
			for (int i=0; i<inlets.Length; i++)
			{
				IFnInlet<object> inlet = inlets[i];
				object product = data.ReadInletProduct(inlet);

				IFnEnter<object> fnEnter = (IFnEnter<object>)inlet.GetInternalPortal(subGraph); 
				subData.StoreProduct(fnEnter, product);
				subData.MarkReady(fnEnter.Id);
			}

			//generating
			if (stop!=null && stop.stop) return;
			subGraph.Generate(subData, stop:stop, ovd:ovd); //with fn override, not graph defaults

			//returning products back from sub-graph exists
			if (stop!=null && stop.stop) return;
			for (int o=0; o<outlets.Length; o++)
			{
				IFnOutlet<object> outlet = outlets[o];
				IFnExit<object> fnExit = (IFnExit<object>)outlet.GetInternalPortal(subGraph);
				object product = subData.ReadInletProduct(fnExit);

				data.StoreProduct(outlet, product);
			}
		}


		public void ClearAny (Generator gen, TileData data)
		/// Called at top level graph each time any node changes
		/// Iterating in sub-graph is done with this
		{
			if (subGraph == null) return;
			TileData subData = data.CreateLoadSubData(id);

			subGraph.ClearGenerator(gen, subData);
		}


		public void ClearDirectly (TileData data)
		/// If changed directly (subgraph exposed values) - resetting generators with exposed values (and relevants if fn used as biome)
		{
			if (subGraph == null) return;
			TileData subData = data.CreateLoadSubData(id);

			//TODO: reset only changed generators
			foreach (IUnit expUnit in subGraph.exposed.AllUnits(subGraph))
				subData.ClearReady((Generator)expUnit);

			foreach (Generator relGen in subGraph.RelevantGenerators(data.isDraft)) 
				subData.ClearReady(relGen);
		}


		public void ClearRecursive (TileData data)
		/// Called by graph on clearing recursive (no matter ready or not). 
		/// Inlets are already cleared to this moment
		{
			if (subGraph == null) return;
			TileData subData = data.CreateLoadSubData(id);

			//first if inlet changed - resetting internal fn fnEnters nodes (and relevants if fn used as biome)			
			foreach (IFnInlet<object> inlet in inlets)
				if (!data.IsReady(inlet.LinkedOutletId))
				{
					IFnPortal<object> fnPortal = inlet.GetInternalPortal(subGraph); 
					if (fnPortal != null  &&  fnPortal is IFnEnter<object> fnEnter) 
						subData.ClearReady(fnEnter.Id);
				}

			//MAIN
			//then calling ClearGenerator on sub-grpah recursively. This will process change in sub-grpah nodes as well
			subGraph.ClearChanged(subData); 

			//finally if any internal exit portal (or relevant) not ready - resetting this fn
			foreach (Generator rel in subGraph.RelevantGenerators(data.isDraft)) //subGraph.GeneratorsOfType<IFnExit<object>>()) 
			{
				if (!subData.IsReady(rel))
					data.ClearReady(this);
			}
		}


/*		public bool CheckClear (TileData data)
		/// Called when clearing recursively from graph
		/// Called once, but no matter generator is ready or not
		{
			if (subGraph == null)
				return true;

			TileData subData = data.GetSubData(id);
			if (subData == null) return true;

			bool ready = true;

			//if inlet changed - portals
			bool inletsReady = true;
			foreach (IFnInlet<object> inlet in inlets)
			{
				ulong linkedId = inlet.LinkedGenId;
				if (linkedId == 0)
					continue; //not connected

				if (!data.IsReady(linkedId)) //should have inlet state changed at the moment
				{
					//enters
					IFnEnter<object> fnEnter = (IFnEnter<object>)inlet.GetInternalPortal(subGraph); 
					if (fnEnter==null) continue;

					subData.ClearReady((Generator)fnEnter);

					inletsReady = false;
					ready = false;
				}
			}

			//and if inlet changed - resetting relevants if function is used as biome
			if (!inletsReady)
				foreach (Generator relGen in subGraph.RelevantGenerators(data.isDraft)) 
				{
					if (relGen is ICustomClear cgen) cgen.ForceClear(subData);
					subData.ClearReady(relGen);
				}

			//proceeding cleared fn enters and checking if any of the layers has internal graph changed
//			if (!subGraph.ClearChanged(subData))  //false if any internal relevant/dependent gen not ready
//				ready = false;

			return ready;
		}


		public void ForceClear (TileData data)
		/// Called when this gen changed directly
		{
			if (subGraph == null)
				return;

			TileData subData = data.GetSubData(id);
			if (subData == null) return;

			//resetting exposed generators
			//TODO: reset only changed generators
			foreach (IUnit expUnit in subGraph.exposed.AllUnits(subGraph))
			{
				if (expUnit is ICustomClear cgen) cgen.ForceClear(subData);
				subData.ClearReady((Generator)expUnit);
			}

			//resetting relevants if function is used as biome
			foreach (Generator relGen in subGraph.RelevantGenerators(data.isDraft)) 
			{
				if (relGen is ICustomClear cgen) cgen.ForceClear(subData);
				subData.ClearReady(relGen);
			}
		}*/
	}

	[GeneratorMenu (name ="Function Outdated", iconName="GeneratorIcons/Function", priority = 1, colorType = typeof(IBiome), disabled = true)]
	public class Function200 : Generator
	{
		public Graph srcGraph;

		public override void Generate (TileData data, StopToken stop) { }

		public Function210 Update ()
		{
			Function210 fn = Generator.Create(typeof(Function210)) as Function210;
			fn.subGraph = srcGraph;
			return fn;
		}
	}
}
