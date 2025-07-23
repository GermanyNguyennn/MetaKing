using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaKing.ViewModels.Contents
{
    public class CouponCreateRequestValidator : AbstractValidator<CouponCreateRequest>
    {
        public CouponCreateRequestValidator()
        {
            RuleFor(x => x.CouponCode).NotEmpty().WithMessage(string.Format(Messages.Required, "Mã giảm giá"));
        }
    }
}
