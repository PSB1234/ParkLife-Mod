using Game.Areas;
using Game.Policies;
using Game.Prefabs;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ParkLife.Systems
{

  [ComponentMenu("Areas/", new Type[] { })]
  public class ParkLifePrefab : AreaPrefab
  {
    public Color m_NameColor = Color.white;
    // This is assigned by ParkLifePrefabSystem when the prefab is registered.
    public ParkLifeUI m_UIObject;
    public Color m_SelectedNameColor = new Color(0.5f, 0.75f, 1f, 1f);

    public override void GetPrefabComponents(HashSet<ComponentType> components)
    {
      base.GetPrefabComponents(components);
      // The game only has fixed area geometry categories. DistrictData makes
      // this use the district-shaped drawing rules instead of Surface area rules.
      components.Add(ComponentType.ReadWrite<DistrictData>());
      components.Add(ComponentType.ReadWrite<ParkLifeData>());
      components.Add(ComponentType.ReadWrite<AreaNameData>());
      components.Add(ComponentType.ReadWrite<AreaGeometryData>());
    }

    public override void GetArchetypeComponents(HashSet<ComponentType> components)
    {
      base.GetArchetypeComponents(components);
      components.Add(ComponentType.ReadWrite<ParkLife>());
      // The built-in area systems expect every district-shaped polygon to
      // carry these base components. ParkLife stores its own rules separately
      // but still uses the district drawing/area engine for geometry.
      components.Add(ComponentType.ReadWrite<District>());
      components.Add(ComponentType.ReadWrite<Geometry>());
      components.Add(ComponentType.ReadWrite<LabelExtents>());
      components.Add(ComponentType.ReadWrite<LabelVertex>());
      components.Add(ComponentType.ReadWrite<ParkLifeModifier>());
      components.Add(ComponentType.ReadWrite<DistrictModifier>());
      components.Add(ComponentType.ReadWrite<Policy>());
    }

    public override void Initialize(EntityManager entityManager, Entity entity)
    {
      base.Initialize(entityManager, entity);
      AreaNameData componentData = default(AreaNameData);
      componentData.m_Color = m_NameColor;
      componentData.m_SelectedColor = m_SelectedNameColor;
      entityManager.SetComponentData(entity, componentData);
    }
  }

}
