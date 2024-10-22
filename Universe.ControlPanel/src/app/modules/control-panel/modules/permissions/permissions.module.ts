import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

import { PermissionsComponent } from "./permissions.component";
import { CorePermissions } from "../../static-data";
import { RightsRequirementGuard } from "../../../../guards";


@NgModule({
	imports: [
		RouterModule.forChild([
			{
				path: "",
				component: PermissionsComponent,
				data: { permissions: CorePermissions.ManagePermission },
				canActivate: [ RightsRequirementGuard ],
			}
		])
	],
	declarations: [ PermissionsComponent ]
})
export class PermissionsModule {}