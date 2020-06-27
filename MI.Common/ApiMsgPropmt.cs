using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Common
{
    /// <summary>
    ///     操作用户信息提示
    /// </summary>
    public enum ApiMsgPropmt
    {
        /// <summary>
        ///     成功
        /// </summary>
        Success = 1011000,


        /// <summary>
        ///     您输入的姓名格式有误！
        /// </summary>
        TrueNameFormat = 1011001,

        /// <summary>
        ///     您输入的手机号格式有误！
        /// </summary>
        MobilePhoneNoFormat = 1011002,

        /// <summary>
        ///     您输入的邮箱地址格式有误！
        /// </summary>
        EmailFormat = 1011003,

        /// <summary>
        ///     您输入的证件号码格式有误！
        /// </summary>
        IdCardNoFormat = 1011004,

        /// <summary>
        ///     您输入的姓名为空！
        /// </summary>
        TrueNameEmpty = 1011005,

        /// <summary>
        ///     您输入的手机号为空！
        /// </summary>
        MobilePhoneNoEmpty = 1011006,

        /// <summary>
        ///     您输入的密码为空！
        /// </summary>
        PasswordEmpty = 1011007,

        /// <summary>
        ///     您输入的确认密码为空！
        /// </summary>
        ComfirmPasswordFormat = 1011008,

        /// <summary>
        ///     两次输入密码不一致！
        /// </summary>
        TwoPasswordDifferent = 1011009,

        /// <summary>
        ///     您输入的证件号码为空！
        /// </summary>
        IdCardNoEmpty = 1011010,

        /// <summary>
        ///     请输入您的账户名！
        /// </summary>
        LoginNameEmpty = 1011011,

        /// <summary>
        ///     请输入您的密码！
        /// </summary>
        LoginPasswordEmpty = 1011012,

        /// <summary>
        ///     原密码错误！
        /// </summary>
        PasswordError = 1011013,

        /// <summary>
        ///     修改失败！
        /// </summary>
        UpdateFailed = 1011014,

        /// <summary>
        ///     证件号码已存在！
        /// </summary>
        IdCardNoExists = 1011015,

        /// <summary>
        ///     手机号已存在！
        /// </summary>
        MobilePhoneNoExists = 1011016,

        /// <summary>
        ///     邮箱地址已存在！
        /// </summary>
        EmailExists = 1011017,

        /// <summary>
        ///     您输入的登录名或密码有误，请重新输入！
        /// </summary>
        LoginOrPasswordError = 1011018,

        /// <summary>
        ///     抱歉，您登录的账户信息有误，请电话客服400-888-6608！
        /// </summary>
        LoginError = 1011019,

        /// <summary>
        ///     登录密码出错，已达上限将锁定密码24小时请找回登录密码后登录！
        /// </summary>
        LoginGtFive = 1011020,

        /// <summary>
        ///     请输入您的新密码！
        /// </summary>
        NewPassword = 1011021,

        /// <summary>
        ///     请输入您的原密码！
        /// </summary>
        OldPassword = 1011022,

        /// <summary>
        ///     程序异常
        /// </summary>
        Exception = 1011023,

        /// <summary>
        ///     请输入6-18位密码！
        /// </summary>
        PasswordFormat = 1011024,

        /// <summary>
        ///     您的账户信息有误！
        /// </summary>
        AccountError = 1011025,

        /// <summary>
        ///     校验码发送失败,请重试！
        /// </summary>
        SendAuthCodeError = 1011026,

        /// <summary>
        ///     抱歉,每天最多只能发送5次校验码！
        /// </summary>
        SendAuthCodeExceFive = 1011027,

        /// <summary>
        ///     抱歉,您输入的验证码不存在！
        /// </summary>
        AuthCodeNotExist = 1011028,

        /// <summary>
        ///     抱歉,您输入的验证码已经过期！
        /// </summary>
        AuthCodeExpire = 1011029,

        /// <summary>
        ///     请输入您的邮箱地址！
        /// </summary>
        EmailEmpty = 1011030,

        /// <summary>
        ///     您输入的邮箱地址不存在！
        /// </summary>
        EmailNoExists = 1011031,

        /// <summary>
        ///     您输入的手机号码不存在！
        /// </summary>
        MobilePhoneNoNoExists = 1011032,

        /// <summary>
        ///     信息发送失败！
        /// </summary>
        SendFail = 1011033,

        /// <summary>
        ///     抱歉，您提交的信息异常！
        /// </summary>
        DataException = 1011034,

        /// <summary>
        ///     请填写您租车人姓名！
        /// </summary>
        FrequentContactNameEmpty = 1011035,

        /// <summary>
        ///     请填写您租车人手机号！
        /// </summary>
        FrequentContactMobilePhoneNoEmpty = 1011036,

        /// <summary>
        ///     请填写您的租车人证件号码！
        /// </summary>
        FrequentContactCardNoEmpty = 1011037,

        /// <summary>
        ///     抱歉,最多只允许添加5条数据信息!
        /// </summary>
        FiveDataLimit = 1011038,

        /// <summary>
        ///     请填写您的发票抬头！
        /// </summary>
        InvoiceTitleEmpty = 1011039,

        /// <summary>
        ///     请填写您的发票税号！
        /// </summary>
        InvoiceTaxpayerIdEmpty = 1011040,

        /// <summary>
        ///     请填写您的收件人名称
        /// </summary>
        InvoiceRecipientEmpty = 1011041,

        /// <summary>
        ///     请填写您的发票邮编！
        /// </summary>
        InvoiceZipCodeEmpty = 1011042,

        /// <summary>
        ///     请填写您的发票地址！
        /// </summary>
        InvoiceAddressEmpty = 1011043,

        /// <summary>
        ///     您输入的邮编格式不正确！
        /// </summary>
        InvoiceZipCodeFormat = 1011044,

        /// <summary>
        ///     请填写您发票收件人的手机号码！
        /// </summary>
        InvoiceMobilePhoneNoEmpty = 1011045,

        /// <summary>
        ///     抱歉,此优惠码已经被绑定！
        /// </summary>
        PromotionCodeExistsBind = 1011046,

        /// <summary>
        ///     抱歉,您输入的编码不存在！
        /// </summary>
        PromotionCodeNotExists = 1011047,

        /// <summary>
        ///     抱歉,您的剩余积分不足以兑换此道具！
        /// </summary>
        PropNotExchange = 1011048,

        /// <summary>
        ///     抱歉,您兑换的道具不存在！
        /// </summary>
        PropNotExists = 1011049,

        /// <summary>
        ///     请选择您的省份！
        /// </summary>
        ProvinceEmpty = 1011050,

        /// <summary>
        ///     请选择您的城市！
        /// </summary>
        CityEmpty = 1011051,

        /// <summary>
        ///     抱歉,您输入的优惠码已经过期！
        /// </summary>
        PromotionCodeExpire = 1011052,

        /// <summary>
        ///     邮箱已激活！
        /// </summary>
        EmailActivated = 1011053,
        /// <summary>
        ///     抱歉,您的账号尚未激活！
        /// </summary>
        LoginNotActive = 1011054,

        /// <summary>
        ///     不是有效的活动
        /// </summary>
        UnExistingActivity = 1011055,

        /// <summary>
        ///     已经参与过此活动
        /// </summary>
        OnlyOnceLimit = 1011056,

        /// <summary>
        ///     抱歉，兑换失败
        /// </summary>
        ForFailure = 1011058,

        /// <summary>
        ///     抱歉,您兑换的道具不存在
        /// </summary>
        PropNoExists = 1011059,

        /// <summary>
        ///     抱歉，此道具已抢兑完毕！
        /// </summary>
        PropNotStock = 1011060,

        /// <summary>
        ///     抱歉,不能添加自己为租车人!
        /// </summary>
        FrequentContactCardNoError = 1011061,

        /// <summary>
        ///     抱歉，由于您未满18周岁，我们无法为您提供租车服务！
        /// </summary>
        AgeLimit = 1011062,

        /// <summary>
        ///     <remarks>该优惠编码已经绑定到您的账户中！</remarks>
        /// </summary>
        PromotionCodeHasBind = 1011063,

        /// <summary>
        ///     保留
        /// </summary>
        Undefined = 10110500,

        /// <summary>
        ///     参数对象不能为Null
        /// </summary>
        ParaNullException = 10110501,

        /// <summary>
        ///     不支持嗨卡充值
        /// </summary>
        NonsupportHicardRecharge = 10110502,

        /// <summary>
        ///     储值账号不能为空
        /// </summary>
        AccountIdEmpty = 10110503,

        /// <summary>
        ///     无效的储值账号
        /// </summary>
        InconformityAccountId = 10110504,

        /// <summary>
        ///     交易关联码不能为空
        /// </summary>
        TranReferCodeEmpty = 10110505,

        /// <summary>
        ///     交易金额必须大于0
        /// </summary>
        AmountMinLimit = 10110506,

        /// <summary>
        ///     未找到储值账号
        /// </summary>
        UnknowAccount = 10110507,

        /// <summary>
        ///     嗨卡卡号不能为空
        /// </summary>
        HicardNumEmpty = 10110508,

        /// <summary>
        ///     嗨卡密码不能为空
        /// </summary>
        HicardPasswordEmpty = 10110509,

        /// <summary>
        ///     用户的Id不能为空
        /// </summary>
        UserIdEmpty = 10110510,

        /// <summary>
        ///     用户名不能为空
        /// </summary>
        UserNameEmpty = 10110511,

        /// <summary>
        ///     用户手机号不能为空
        /// </summary>
        UserCellphoneEmpty = 10110512,

        /// <summary>
        ///     交易流水号不能为空
        /// </summary>
        TranNoEmpty = 10110513,

        /// <summary>
        ///     操作人不能为空
        /// </summary>
        OprNoEmpty = 10110514,

        /// <summary>
        ///     未知的嗨卡卡号
        /// </summary>
        UnknownHicardNum = 10110515,

        /// <summary>
        ///     嗨卡密码不正确
        /// </summary>
        HicardPasswordWrong = 10110516,

        /// <summary>
        ///     嗨卡已过期
        /// </summary>
        HicardExpired = 10110517,

        /// <summary>
        ///     嗨卡已被使用
        /// </summary>
        HicardUsed = 10110518,

        /// <summary>
        ///     储值卡未激活，请通知财务激活储值卡
        /// </summary>
        HicardUnsaled = 10110519,

        /// <summary>
        ///     储值卡序列号不正确, 或者已经被使用
        /// </summary>
        HicardUnknownError = 10110520,

        /// <summary>
        ///     无效的交易流水号
        /// </summary>
        InconformityTranNo = 10110521,

        /// <summary>
        ///     流水号不存在或已被撤销
        /// </summary>
        UnknownTranNo = 10110522,

        /// <summary>
        ///     用户标识不能为空
        /// </summary>
        UserMarkEmpty = 10110523,

        /// <summary>
        ///     不能重复注册
        /// </summary>
        AccountExisted = 10110524,

        /// <summary>
        ///     未找到用户信息
        /// </summary>
        CannotFindAccountInfo = 10110525,

        /// <summary>
        ///     请至少输入一个参数
        /// </summary>
        ParaShouldMoreThanOne = 10110526,

        /// <summary>
        ///     未找到相关数据
        /// </summary>
        QueryDataEmpty = 10110527,

        /// <summary>
        ///     余额不足
        /// </summary>
        BalanceInsufficient = 10110528,

        /// <summary>
        ///     身份证信息有误
        /// </summary>
        IdCardError = 10110530,

        /// <summary>
        /// 提交内容已存在
        /// </summary>
        ContentExist = 10110531,

        /// <summary>
        /// 企业ID不能为空
        /// </summary>
        EnterpriseIDEmpty = 10110532,

        /// <summary>
        /// 储值账号创建失败
        /// </summary>
        CreateChargeAccountError = 10110533,

        /// <summary>
        /// 审核通过的发票，30天内不能再次修改。
        /// </summary>
        CheckInvoiceNotNodify = 10110535,

        /// <summary>
        /// 收件人长度不能超过10位
        /// </summary>
        RecipientLengthOver = 10110536,

        /// <summary>
        /// 请填写开户行！
        /// </summary>
        BankEmpty = 10110537,

        /// <summary>
        /// 请填写银行账号！
        /// </summary>
        BankAccountEmpty = 10110538,

        /// <summary>
        /// 请填写发票手机号码!
        /// </summary>
        InvoicePhoneEmpty = 10110539,

        /// <summary>
        /// 发票税号填写错误!
        /// </summary>
        InvoiceNumberError = 10110540,

        /// <summary>
        /// 已经存在增票，不能重复添加
        /// </summary>
        AddInvoiceExist = 10110541,

        /// <summary>
        /// 消费积分失败
        /// </summary>
        UsePointError = 10110542,

        /// <summary>
        /// 积分不能小于0
        /// </summary>
        PointNotPositive = 10110543,

        /// <summary>
        /// 积分不足
        /// </summary>
        PointShortage = 10110544,

        /// <summary>
        /// 手机号信息不一致
        /// </summary>
        PhoneNumberNotMatch = 10110545,

        /// <summary>
        /// 身份证信息不一致
        /// </summary>
        IdCardNotMatch = 10110546,

        /// <summary>
        /// 渠道ID不能为空
        /// </summary>
        ChannelIDEmpty = 10110547,

        /// <summary>
        /// 您在一嗨租车曾使用的姓名为：{0}，请确保姓名一致。
        /// </summary>
        CheckChannelRegisterName = 10110548,

        /// <summary>
        /// 该证件号已通过验证，不支持修改。
        /// </summary>
        CheckIdCardNotModify = 10110549,

        /// <summary>
        /// 验证码不能为空
        /// </summary>
        AuthCodeEmpty = 10110550,

        /// <summary>
        /// 驾驶证号长度不符（6-18）
        /// </summary>
        LicenseNumberOutOfRange = 10110551,

        /// <summary>
        /// 驾驶证编号长度不符（12）
        /// </summary>
        FileNumberOutOfRange = 10110552,

        /// <summary>
        /// 紧急联系人手机长度不能超过20位
        /// </summary>
        ContactPhoneNumberOverLength = 10110553,

        /// <summary>
        /// 紧急联系人地址长度不能超过150位
        /// </summary>
        ContactAddressOverLength = 10110554,

        /// <summary>
        /// 证件号长度不符（6-18）
        /// </summary>
        CardNumberOutOfRange = 10110555,

        /// <summary>
        /// 消费金额不能为负
        /// </summary>
        ConsumptionMinLimit = 10110556,

        /// <summary>
        /// 用车天数必须大于0
        /// </summary>
        UsedCarDaysMinLimit = 10110557,

        /// <summary>
        /// 已验证单数必须大于0
        /// </summary>
        VerifyOrdersAmount = 10110558,

        /// <summary>
        /// 注册邀请码过长
        /// </summary>
        InviteCodeLengthOver = 10110559,

        /// <summary>
        /// 新用户有预约中的订单不允许修改证件号
        /// </summary>
        HaveReservationOrderNotModifyIdCard = 10110560,

        /// <summary>
        /// 您的手机号已经存在于一嗨账户，姓名为：{0}。数据库存的是{0}直接返回姓名
        /// </summary>
        CtripRegisterMobileExist = 10110561,

        /// <summary>
        /// 请先完成实名认证再添加常用租车人
        /// </summary>
        AddCustomerDriverNeedValidation = 10110562,

        /// <summary>
        /// 工号不能为空
        /// </summary>
        WorkNumberIsEmpty = 10110563,

        /// <summary>
        /// 企业Id不能为空Corporate ID can't be empty
        /// </summary>
        CorporateIDIsEmpty = 10110564,

        /// <summary>
        /// 您输入的手机号已注册，请直接登录
        /// </summary>
        PhoneNumberIsRegisted = 10110565,

        /// <summary>
        /// 证件号已经验证或者有预约中的自驾订单，不允许修改姓名
        /// </summary>
        IdNumberHasBeenVerified = 10110566,

        /// <summary>
        /// 您已经添加过该证件号为您的常用租车人，请先删除常用租车人的信息
        /// </summary>
        IdNumberInYourRegularCarbokerList = 10110567,


        /// <summary>
        ///     港澳居民来往内地通行证格式为9位字母或数字
        /// </summary>
        MacaoPermitFormat = 10110568,


        /// <summary>
        ///     台胞证格式为8位字母或数字
        /// </summary>
        TaiwanCompatriotFormat = 10110569,

    }
}
