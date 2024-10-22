import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

import { GridModule } from "@features/grid";
import { UsersComponent } from "./users.component";
import { CorePermissions } from "../../static-data";
import { RightsRequirementGuard } from "../../../../guards";


@NgModule({
	imports: [
		GridModule,

		RouterModule.forChild([
			{
				path: "",
				component: UsersComponent,
				data: { permissions: CorePermissions.ManageUser },
				canActivate: [ RightsRequirementGuard ],
			}
		])
	],
	declarations: [ UsersComponent ]
})
export class UsersModule {}