namespace DotnetSpider.Enterprise.Core.Entities
{
	public interface IEntity : IEntity<int>
	{
	}

	public interface IEntity<TPrimaryKey>
	{
		/// <summary>
		/// Unique identifier for this entity.
		/// </summary>
		TPrimaryKey Id { get; set; }
	}

	/// <summary>
	/// 泛型实体基类
	/// </summary>
	/// <typeparam name="TPrimaryKey">主键类型</typeparam>
	public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
	{
		/// <summary>
		/// 主键
		/// </summary>
		public virtual TPrimaryKey Id { get; set; }
	}
}
