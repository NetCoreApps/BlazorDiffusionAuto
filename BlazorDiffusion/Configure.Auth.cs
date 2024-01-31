using ServiceStack.Auth;
using BlazorDiffusion.ServiceModel;
using BlazorDiffusion.ServiceInterface;
using ServiceStack.Html;

[assembly: HostingStartup(typeof(BlazorDiffusion.ConfigureAuth))]

namespace BlazorDiffusion;

public class ConfigureAuth : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) =>
        {
            services.AddPlugin(new AuthFeature(IdentityAuth.For<AppUser,int>(options => {
                options.EnableCredentialsAuth = true;
                options.SessionFactory = () => new CustomUserSession();
                
                options.CredentialsAuth();

                options.AdminUsersFeature(feature =>
                {
                    feature.FormLayout =
                    [
                        Input.For<AppUser>(x => x.UserName, c => c.FieldsPerRow(2)),
                        Input.For<AppUser>(x => x.Email, c => {
                            c.Type = Input.Types.Email;
                            c.FieldsPerRow(2);
                        }),
                        Input.For<AppUser>(x => x.FirstName, c => c.FieldsPerRow(2)),
                        Input.For<AppUser>(x => x.LastName, c => c.FieldsPerRow(2)),
                        Input.For<AppUser>(x => x.DisplayName, c => c.FieldsPerRow(2)),
                        Input.For<AppUser>(x => x.PhoneNumber, c =>
                        {
                            c.Type = Input.Types.Tel;
                            c.FieldsPerRow(2);
                        }),
                    ];
                    feature.QueryIdentityUserProperties =
                    [
                        nameof(AppUser.Id),
                        nameof(AppUser.DisplayName),
                        nameof(AppUser.Email),
                        nameof(AppUser.UserName),
                        nameof(AppUser.LockoutEnd),
                    ];
                    feature.DefaultOrderBy = nameof(AppUser.DisplayName);
                    feature.SearchUsersFilter = (q, query) =>
                    {
                        var queryUpper = query.ToUpper();
                        q = int.TryParse(query, out var idVal)
                            ? q.Where(x => x.Id == idVal)
                            : q.Where(x =>
                                x.DisplayName!.Contains(query) ||
                                x.NormalizedUserName!.Contains(queryUpper) ||
                                x.NormalizedEmail!.Contains(queryUpper));
                        return q;
                    };
                });
            })));
        });
}
