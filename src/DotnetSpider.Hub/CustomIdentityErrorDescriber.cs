using Microsoft.AspNetCore.Identity;

namespace DotnetSpider.Hub
{
	internal class CustomIdentityErrorDescriber : IdentityErrorDescriber
	{
        /// <summary>
        /// 默认错误
        /// </summary>
        /// <returns></returns>
		public override IdentityError DefaultError() { return new IdentityError { Code = nameof(DefaultError), Description = $"未知的错误。" }; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IdentityError ConcurrencyFailure() { return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "Optimistic concurrency failure, object has been modified." }; }
        /// <summary>
        /// 密码不匹配
        /// </summary>
        /// <returns></returns>
        public override IdentityError PasswordMismatch() { return new IdentityError { Code = nameof(PasswordMismatch), Description = "密码不正确。" }; }
        /// <summary>
        /// 无效令牌
        /// </summary>
        /// <returns></returns>
        public override IdentityError InvalidToken() { return new IdentityError { Code = nameof(InvalidToken), Description = "验证码不正确。" }; }
        /// <summary>
        /// 重复登录
        /// </summary>
        /// <returns></returns>
        public override IdentityError LoginAlreadyAssociated() { return new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "用户已经登陆。" }; }
        /// <summary>
        /// 无效用户名
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override IdentityError InvalidUserName(string userName) { return new IdentityError { Code = nameof(InvalidUserName), Description = $"用户名 '{userName}' 不合法。" }; }
        /// <summary>
        /// 无效邮箱名
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public override IdentityError InvalidEmail(string email) { return new IdentityError { Code = nameof(InvalidEmail), Description = $"邮件 '{email}' 不合法。" }; }
        /// <summary>
        /// 用户名重复
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override IdentityError DuplicateUserName(string userName) { return new IdentityError { Code = nameof(DuplicateUserName), Description = $"用户名 '{userName}' 已经被使用。" }; }
        /// <summary>
        /// 邮箱名重复
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public override IdentityError DuplicateEmail(string email) { return new IdentityError { Code = nameof(DuplicateEmail), Description = $"邮件 '{email}' 已经被使用。" }; }
        /// <summary>
        /// 角色名无效
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public override IdentityError InvalidRoleName(string role) { return new IdentityError { Code = nameof(InvalidRoleName), Description = $"角色名 '{role}' 不合法。" }; }

        /// <summary>
        /// 角色名重复
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public override IdentityError DuplicateRoleName(string role) { return new IdentityError { Code = nameof(DuplicateRoleName), Description = $"角色名 '{role}' 已经被使用。" }; }
        /// <summary>
        /// 用户已设置了密码
        /// </summary>
        /// <returns></returns>
        public override IdentityError UserAlreadyHasPassword() { return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "User already has a password set." }; }
        /// <summary>
        /// 用户已被禁用
        /// </summary>
        /// <returns></returns>
        public override IdentityError UserLockoutNotEnabled() { return new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "Lockout is not enabled for this user." }; }
        /// <summary>
        /// 用户已加入角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public override IdentityError UserAlreadyInRole(string role) { return new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"User already in role '{role}'." }; }
        /// <summary>
        /// 用户没有加入角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public override IdentityError UserNotInRole(string role) { return new IdentityError { Code = nameof(UserNotInRole), Description = $"User is not in role '{role}'." }; }
        /// <summary>
        /// 密码太短
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public override IdentityError PasswordTooShort(int length) { return new IdentityError { Code = nameof(PasswordTooShort), Description = $"密码最少需要 {length} 个字符。" }; }
        /// <summary>
        /// 密码没有特殊字母
        /// </summary>
        /// <returns></returns>
        public override IdentityError PasswordRequiresNonAlphanumeric() { return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "密码需要包含特殊字符。" }; }
        /// <summary>
        /// 密码没有数字
        /// </summary>
        /// <returns></returns>
        public override IdentityError PasswordRequiresDigit() { return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "密码需要包含数字 ('0'-'9')." }; }
        /// <summary>
        /// 密码没有小写字母
        /// </summary>
        /// <returns></returns>
        public override IdentityError PasswordRequiresLower() { return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "密码至少需要一个小写英文字母 ('a'-'z')." }; }
        /// <summary>
        /// 密码没有大写字母
        /// </summary>
        /// <returns></returns>
        public override IdentityError PasswordRequiresUpper() { return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "密码至少需要一个大写英文字母 ('A'-'Z')." }; }
	}
}