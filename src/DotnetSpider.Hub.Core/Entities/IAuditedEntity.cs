using System;

namespace DotnetSpider.Hub.Core.Entities
{
	public interface IAuditedEntity
	{
		/// <summary>
		/// Last modification date of this entity.
		/// </summary>
		DateTime? LastModificationTime { get; set; }

		/// <summary>
		/// Last modifier user of this entity.
		/// </summary>
		long? LastModifierUserId { get; set; }

		/// <summary>
		/// Creation time of this entity.
		/// </summary>
		DateTime CreationTime { get; set; }

		/// <summary>
		/// Creator of this entity.
		/// </summary>
		long? CreatorUserId { get; set; }
	}

	public abstract class AuditedEntity<TPrimaryKey> : Entity<TPrimaryKey>, IAuditedEntity
	{
		public virtual DateTime? LastModificationTime { get; set; }
		public virtual long? LastModifierUserId { get; set; }
		public virtual DateTime CreationTime { get; set; }
		public virtual long? CreatorUserId { get; set; }
	}

	/// <summary>
	/// 定义默认主键类型为long的实体基类
	/// </summary>
	public abstract class AuditedEntity : AuditedEntity<long>
	{
	}
}
