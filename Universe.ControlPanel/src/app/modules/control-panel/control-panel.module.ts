import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

import { AuthorizedGuard, RightsRequirementGuard } from "../../guards";
import { ControlPanelLayoutComponent } from "./components/control-panel-layout.component";
import { AppLayoutModule } from "./modules/sakai-ng-layout";
import { ProjectDefaults, ProjectRoutes } from "../project";
import { CorePermissions } from "./static-data";


@NgModule({
	declarations: [ ControlPanelLayoutComponent ],
	imports: [
		AppLayoutModule,
		RouterModule.forChild([
			{
				path: "",
				canActivate: [ AuthorizedGuard ],
				canActivateChild: [ AuthorizedGuard ],
				component: ControlPanelLayoutComponent,
				children: [
					{ path: "", redirectTo: ProjectDefaults.AuthorizedRoute, pathMatch: "full" },
					{
						path: "users",
						data: { permissions: CorePermissions.ManageUser },
						canLoad: [ RightsRequirementGuard ],
						loadChildren: () => import("./modules/users/users.module")
							.then(x => x.UsersModule)
					},
					{
						path: "roles",
						data: { permissions: CorePermissions.ManageRole },
						canLoad: [ RightsRequirementGuard ],
						loadChildren: () => import("./modules/roles/roles.module")
							.then(x => x.RolesModule)
					},
					{
						path: "permissions",
						data: { permissions: CorePermissions.ManagePermission },
						canLoad: [ RightsRequirementGuard ],
						loadChildren: () => import("./modules/permissions/permissions.module")
							.then(x => x.PermissionsModule)
					},
					{
						path: "profile",
						loadChildren: () => import("./modules/profile/profile.module")
							.then(x => x.ProfileModule)
					},
					{
						path: "app-configuration",
						data: { permissions: CorePermissions.ManageAppConfiguration },
						canLoad: [ RightsRequirementGuard ],
						loadChildren: () => import("./modules/app-configuration/app-configuration.module")
							.then(x => x.AppConfigurationModule)
					},
					{
						path: "audit",
						data: { permissions: CorePermissions.ReadAuditData },
						canLoad: [ RightsRequirementGuard ],
						loadChildren: () => import("./modules/audit/audit.module")
							.then(x => x.AuditModule)
					},
					...ProjectRoutes
				]
			}
		])
	]
})
export class ControlPanelModule {}