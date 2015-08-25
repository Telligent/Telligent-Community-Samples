using System;
using System.Collections.Generic;
using Telligent.Evolution.Components;

namespace Telligent.Evolution.Extensions.TableOfContents.Tests.Mocks
{
	public class DummySecurityService : ISecurityService
	{
		public ISecurityCanDoesOrGetRolesForSyntax For(ISecuredItem item)
		{
			throw new NotImplementedException();
		}

		public PermissionList GetEffectivePermissions(Guid nodeId)
		{
			throw new NotImplementedException();
		}

		public PermissionList GetImmediatePermissions(Guid nodeId)
		{
			throw new NotImplementedException();
		}

		public PermissionList GetImmediatePermissionsByRole(Guid nodeId, Role role)
		{
			throw new NotImplementedException();
		}

		public PermissionList GetEffectivePermissionsByNodeAndRole(Guid nodeId, Role role)
		{
			throw new NotImplementedException();
		}

		public PermissionList GetEffectivePermissionsByUserAndNode(Guid nodeId, int userId)
		{
			throw new NotImplementedException();
		}

		public void SetPermissions(ISecuredItem item, IEnumerable<PermissionEntry> entries)
		{
			throw new NotImplementedException();
		}

		public void SetPermissionsForRole(ISecuredItem item, int roleId, IEnumerable<PermissionEntry> entries)
		{
			throw new NotImplementedException();
		}

		public void RecalculateAllPermissions()
		{
			throw new NotImplementedException();
		}

		public void RecalculatePermissions(ISecuredItem item)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Role> GetRolesForPermission(ISecuredItem item, Permission permission)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Permission> GetAllPermissions()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Permission> GetPermissionsForRole(GroupType groupType, int roleId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Permission> GetPermissionsForNodeType(NodeType nodeType)
		{
			throw new NotImplementedException();
		}

		public NodeType GetNodeTypeForNode(ISecuredItem item)
		{
			throw new NotImplementedException();
		}

		public bool CheckPermission(User user, ISecuredItem item, Permission permission)
		{
			throw new NotImplementedException();
		}

		public bool CheckAction(User user, ISecuredItem item, SecuredAction actionId)
		{
			throw new NotImplementedException();
		}

		public void UpdateImmediateAction(Role role, ISecuredItem item, SecuredAction action, bool isAllowed)
		{
			throw new NotImplementedException();
		}

		public Guid GetParentNodeId(Guid nodeId)
		{
			throw new NotImplementedException();
		}

		public ICollection<IAdditiveSecurityRule> AdditiveRules
		{
			get { throw new NotImplementedException(); }
		}

		public ICollection<ISubtractiveSecurityRule> SubtractiveRules
		{
			get { throw new NotImplementedException(); }
		}
	}

}
